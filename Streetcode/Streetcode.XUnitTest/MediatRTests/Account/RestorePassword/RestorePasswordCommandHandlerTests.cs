using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Account.RestorePassword;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Account.RestorePassword;

public class RestorePasswordCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly IRequestHandler<RestorePasswordCommand, Result<string>> _handler;

    public RestorePasswordCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _loggerMock = new Mock<ILoggerService>();
        _handler = new RestorePasswordHandler(_userManagerMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnFailResult()
    {
        // Arrange
        var command = CreateCommand("nonexistent_user", "valid-token", "NewPassword123!");
        var erorrMsg = "User not found";
        SetupUserManagerFindByIdAsync(command.ResetPasswordDto.UserId, null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        Assert.Equal(erorrMsg, result.Errors?.FirstOrDefault()?.Message);
    }

    [Fact]
    public async Task Handle_ResetPasswordFails_ShouldReturnFailResultWithErrors()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var command = CreateCommand(user.Id.ToString(), "invalid-token", "NewPassword123!");
        var erorrMsg = "Error reseting password for request: Invalid token";
        SetupUserManagerFindByIdAsync(command.ResetPasswordDto.UserId, user);
        SetupUserManagerResetPasswordAsync(user, command.ResetPasswordDto.Token, command.ResetPasswordDto.NewPassword, IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(erorrMsg, result.Errors.FirstOrDefault()?.Message);
    }

    [Fact]
    public async Task Handle_ResetPasswordSucceeds_ShouldReturnSuccessResult()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var command = CreateCommand(user.Id.ToString(), "valid-token", "NewPassword123!");
        SetupUserManagerFindByIdAsync(command.ResetPasswordDto.UserId, user);
        SetupUserManagerResetPasswordAsync(user, command.ResetPasswordDto.Token, command.ResetPasswordDto.NewPassword, IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Errors.Should().BeEmpty();
    }

    private RestorePasswordCommand CreateCommand(string userId, string token, string newPassword)
    {
        var restoreDto = new RestorePasswordDto
        {
            UserId = userId,
            Token = token,
            NewPassword = newPassword
        };

        return new RestorePasswordCommand(restoreDto);
    }

    private void SetupUserManagerFindByIdAsync(string userId, User user)
    {
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
    }

    private void SetupUserManagerResetPasswordAsync(User user, string token, string newPassword, IdentityResult result)
    {
        _userManagerMock.Setup(x => x.ResetPasswordAsync(user, token, newPassword)).ReturnsAsync(result);
    }
}