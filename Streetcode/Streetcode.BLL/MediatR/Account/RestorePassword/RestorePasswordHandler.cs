using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.RestorePassword;

public class RestorePasswordHandler : IRequestHandler<RestorePasswordCommand, Result<string>>
{
    private readonly UserManager<User> _userManager;
    private readonly ILoggerService _logger;

    public RestorePasswordHandler(UserManager<User> userManager, ILoggerService logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(RestorePasswordCommand request, CancellationToken cancellationToken)
    {
        var resetPasswordDto = request.ResetPasswordDto;

        var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
        if (user == null)
        {
            var errorMsg = ErrorMessages.UserNotFound;
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            var identityErrors = string.Join(", ", result.Errors.Select(e => e.Description));
            var formatErrors = string.Format(ErrorMessages.ErrorResetingPassword, identityErrors);
            _logger.LogError(request, formatErrors);
            return Result.Fail(formatErrors);
        }

        return Result.Ok("Password recovery successful!");
    }
}