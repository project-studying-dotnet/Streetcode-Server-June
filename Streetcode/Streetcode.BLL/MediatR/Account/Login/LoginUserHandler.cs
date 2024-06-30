using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.Login;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<LoginResultDTO>>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(UserManager<User> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResultDTO>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // already created User without password but token is generated
        // "test2.email@com.ua" - username and password
        var user = await _userManager.FindByEmailAsync(request.LoginUser.Login);
        var claims = await _tokenService.GetUserClaimsAsync(user);

        var accessToken = await _tokenService.GenerateAccessToken(user, claims);

        var loginResult = new LoginResultDTO
        {
            AccessToken = accessToken,
            // RefreshToken will be implemented in other story
            RefreshToken = null!
        };

        return Result.Ok(loginResult);
    }
}