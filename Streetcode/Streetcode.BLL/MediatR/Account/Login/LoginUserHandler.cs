using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.Login;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<LoginResultDTO>>
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILoggerService _logger;

    public LoginUserHandler(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IMapper mapper, ILoggerService logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<LoginResultDTO>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // Already created User 
        // UserName = "SuperAdmin"
        // adminPass = "*Superuser18"

        var user = await _userManager.FindByNameAsync(request.LoginUser.Username);

        if (user == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.InvalidUsernameOrPassword, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.LoginUser.Password, false);

        if (!result.Succeeded)
        {

            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.InvalidUsernameOrPassword, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var tokens = await _tokenService.GenerateTokens(user);

        var loginResult = new LoginResultDTO
        {
            User = _mapper.Map<UserDTO>(user),
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };

        return Result.Ok(loginResult);
    }
}
