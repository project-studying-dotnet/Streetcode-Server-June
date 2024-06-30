using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SoftServerCinema.Security.Interfaces;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Account.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<UserDTO>>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public RegisterUserHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger, UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenservice)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenservice;
        }
        //AuthenticationResponseDto - userdto

        public async Task<Result<UserDTO>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var req = request.newUser;
            if (await IsEmailUse(req.Email))
            {
                string errorMessage = "A user with this email is already registered.";
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            User user = _mapper.Map<User>(req);
            
            IdentityResult newUser = await _userManager.CreateAsync(user, req.Password);
            //
            if (!newUser.Succeeded)
            {
                string errorMessage = string.Join(
            Environment.NewLine,
            newUser.Errors.Select(e => e.Description));

                return Result.Fail(errorMessage);
            }

            // sign-in
            await _signInManager.SignInAsync(user, isPersistent: false);

            // add user role
            IdentityResult addingRoleResult = await _userManager.AddToRoleAsync(user, UserRole.Moderator.ToString());

            if (!addingRoleResult.Succeeded)
            {
                string errorMessage = string.Join(
           Environment.NewLine,
           addingRoleResult.Errors.Select(e => e.Description));

                return Result.Fail(errorMessage);
            }

            var claims = await _tokenService.GetUserClaimsAsync(user);

            var response = _tokenService.GenerateAccessToken(user, claims);


            if (await _repositoryWrapper.SaveChangesAsync() <= 0)
            {
                var errorMessage = "Failed to save refresh token.";

                _logger.LogError(response, errorMessage);

                return Result.Fail(errorMessage);
            }

            return Result.Ok();
        }

        private async Task<bool> IsEmailUse(string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);

            return user is not null;
        }
    }
}
