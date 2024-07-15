using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Account.ChangePassword;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.CacheService;
using Streetcode.BLL.Services.CookieService.Interfaces;
using Streetcode.DAL.Entities.Users;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Account.ChangePassword
{
    public class ChangePasswordTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly ChangePasswordHandler _handler;
        private readonly Mock<ICookieService> _cookieServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;

        public ChangePasswordTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            _loggerMock = new Mock<ILoggerService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _cookieServiceMock = new Mock<ICookieService>();
            _cacheServiceMock = new Mock<ICacheService>();
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object, 
                _httpContextAccessorMock.Object, 
                new Mock<IUserClaimsPrincipalFactory<User>>().Object, 
                null,
                null,
                null,
                null);

            _handler = new ChangePasswordHandler(
                _userManagerMock.Object,
                _cacheServiceMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object,
                _httpContextAccessorMock.Object,
                _cookieServiceMock.Object,
                _signInManagerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenNoAccessToken()
        {
            // Arrange
            var command = new ChangePasswordCommand(
                new ChangePasswordDTO 
                { CurrentPassword = "nonexistent", NewPassword = "password", ConfirmPassword = "password" });

            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, command);

            var cookies = new Mock<IRequestCookieCollection>();
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();

            cookies.Setup(c => c.TryGetValue("accessToken", out It.Ref<string?>.IsAny)).Returns(false);
            requestMock.Setup(r => r.Cookies).Returns(cookies.Object);
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.IsFailed);
                Assert.Equal(errorMsg, result.Errors[0].Message);
            });
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenUserNotFound()
        {
            // Arrange
            var command = new ChangePasswordCommand(
                new ChangePasswordDTO
                { CurrentPassword = "nonexistent", NewPassword = "password", ConfirmPassword = "password" });

            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, command);

            var cookies = new Mock<IRequestCookieCollection>();
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            var user = new Mock<User>();           

            cookies.Setup(c => c.TryGetValue("accessToken", out It.Ref<string?>.IsAny)).Returns(true);
            requestMock.Setup(r => r.Cookies).Returns(cookies.Object);
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _tokenServiceMock.Setup(t => t.GetUserIdFromAccessToken(It.IsAny<string>())).Returns("-1");
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.IsFailed);
                Assert.Equal(errorMsg, result.Errors[0].Message);
            });
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenFailToChangePassword()
        {
            // Arrange
            var command = new ChangePasswordCommand(
                new ChangePasswordDTO
                { CurrentPassword = "nonexistent", NewPassword = "password", ConfirmPassword = "password" });

            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToChangePassword, command);

            var cookies = new Mock<IRequestCookieCollection>();
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            var user = new Mock<User>();
            var failedIdentityResult = IdentityResult.Failed(new IdentityError { Description = ErrorMessages.FailToChangePassword });

            cookies.Setup(c => c.TryGetValue("accessToken", out It.Ref<string?>.IsAny)).Returns(true);
            requestMock.Setup(r => r.Cookies).Returns(cookies.Object);
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _tokenServiceMock.Setup(t => t.GetUserIdFromAccessToken(It.IsAny<string>())).Returns("-1");
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
            _userManagerMock.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(failedIdentityResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.IsFailed);
                Assert.Equal(errorMsg, result.Errors[0].Message);
            });
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenFailToBlacklistCookies()
        {
            // Arrange
            var command = new ChangePasswordCommand(
                new ChangePasswordDTO
                { CurrentPassword = "nonexistent", NewPassword = "password", ConfirmPassword = "password" });

            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailedToSetTokenInBlackList, command);

            var cookies = new Mock<IRequestCookieCollection>();
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            var user = new Mock<User>();
            var successIdentityResult = IdentityResult.Success;

            cookies.Setup(c => c.TryGetValue("accessToken", out It.Ref<string?>.IsAny)).Returns(true);
            requestMock.Setup(r => r.Cookies).Returns(cookies.Object);
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _tokenServiceMock.Setup(t => t.GetUserIdFromAccessToken(It.IsAny<string>())).Returns("-1");
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
            _userManagerMock.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(successIdentityResult);
            _cacheServiceMock.Setup(c => c.SetBlacklistedTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.IsFailed);
                Assert.Equal(errorMsg, result.Errors[0].Message);
            });
        }

        [Fact]
        public async Task Handle_ReturnsSuccess_WhenCorrectPassword()
        {
            // Arrange
            var command = new ChangePasswordCommand(
                new ChangePasswordDTO
                { CurrentPassword = "password", NewPassword = "newpassword", ConfirmPassword = "newpassword" });

            var cookies = new Mock<IRequestCookieCollection>();
            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            var user = new Mock<User>();
            var successIdentityResult = IdentityResult.Success;

            cookies.Setup(c => c.TryGetValue("accessToken", out It.Ref<string?>.IsAny)).Returns(true);
            requestMock.Setup(r => r.Cookies).Returns(cookies.Object);
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            _tokenServiceMock.Setup(t => t.GetUserIdFromAccessToken(It.IsAny<string>())).Returns((string?)null);
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
            _userManagerMock.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), command.PasswordChange.CurrentPassword, command.PasswordChange.NewPassword))
                .ReturnsAsync(IdentityResult.Success);
            _cacheServiceMock.Setup(c => c.SetBlacklistedTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
