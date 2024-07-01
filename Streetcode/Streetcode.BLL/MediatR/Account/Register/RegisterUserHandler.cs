using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Account.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<UserDTO>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public RegisterUserHandler(IMapper mapper, ILoggerService logger, UserManager<User> userManager, ITokenService tokenservice)
        {
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenservice;
        }

        public async Task<Result<UserDTO>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var req = request.newUser;
            if (await IsEmailUse(req.Email))
            {
                string errorMessage = "A user with this email is already registered.";
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            if (await IsEmailUse(req.Login))
            {
                string errorMessage = "A user with this login is already registered.";
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            var user = _mapper.Map<User>(req);

            IdentityResult newUser = await _userManager.CreateAsync(user, req.Password);
            
            if (!newUser.Succeeded)
            {
                var errorMessage = "Failed to create user";

                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            IdentityResult addingRoleResult = await _userManager.AddToRoleAsync(user, "USER");

            if (!addingRoleResult.Succeeded)
            {
                var errorMessage = "Failed to add role";

                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            var claims = await _tokenService.GetUserClaimsAsync(user);

            var response = _tokenService.GenerateAccessToken(user, claims);

            return Result.Ok(_mapper.Map<UserDTO>(user));
        }

        private async Task<bool> IsEmailUse(string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);

            return user is not null;
        }
    }
}
