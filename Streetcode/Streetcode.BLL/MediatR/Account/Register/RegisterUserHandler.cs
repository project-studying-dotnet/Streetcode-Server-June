using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.MediatR.Account.Register
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RegisterUserHandler(
        IMapper mapper, 
        ILoggerService logger, 
        UserManager<User> userManager, 
        ITokenService tokenService, 
        IHttpContextAccessor contextAccessor)
    {
        _mapper = mapper;
        _logger = logger;
        _userManager = userManager;
        _tokenService = tokenService;
        _httpContextAccessor = contextAccessor;
    }

    public async Task<Result<UserDTO>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var req = request.newUser;
        var httpContext = _httpContextAccessor.HttpContext;

        if (await IsEmailUse(req.Email))
        {
            var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.EmailIsUse, request);
            _logger.LogError(request, errorMessage);
            return Result.Fail(new Error(errorMessage));
        }

        if (await IsLoginUse(req.Username))
        {
            var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.LoginIsUse, request);
            _logger.LogError(request, errorMessage);
            return Result.Fail(errorMessage);
        }

        var user = _mapper.Map<User>(req);
        IdentityResult newUserResult = await _userManager.CreateAsync(user, req.Password);
        if (!newUserResult.Succeeded)
        {
            var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.FailCreateUser, request);
            _logger.LogError(request, errorMessage);
            return Result.Fail(errorMessage);
        }

        IdentityResult addingRoleResult = await _userManager.AddToRoleAsync(user, UserRole.User.ToString());
        if (!addingRoleResult.Succeeded)
        {
            var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.FailAddRole, request);
            _logger.LogError(request, errorMessage);
            return Result.Fail(errorMessage);
        }

        await _tokenService.GenerateAndSetTokensAsync(user, httpContext!.Response);


        return Result.Ok(_mapper.Map<UserDTO>(user));
    }

    private async Task<bool> IsEmailUse(string email)
    {
        User? user = await _userManager.FindByEmailAsync(email);
        return user is not null;
    }

    private async Task<bool> IsLoginUse(string login)
    {
        User? user = await _userManager.FindByNameAsync(login);
        return user is not null;
    }
}
