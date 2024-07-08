using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Account.Logout;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.CacheService;
using Streetcode.DAL.Entities.Users;
using System.Linq;
using System.Security.Cryptography;
using MockQueryable.Moq;
using Streetcode.BLL.DTO.Users;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Account.Logout
{
    public class LogoutUserHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly LogoutUserHandler _handler;

        public LogoutUserHandlerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            _cacheServiceMock = new Mock<ICacheService>();
            _loggerMock = new Mock<ILoggerService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _tokenServiceMock = new Mock<ITokenService>();

            _handler = new LogoutUserHandler(
                _userManagerMock.Object,
                _cacheServiceMock.Object,
                _loggerMock.Object,
                _httpContextAccessorMock.Object,
                _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenAccessTokenNotFound()
        {
            // Arrange
            var command = new LogoutUserCommand();
            var httpContext = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, command);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenUserUpdateFails()
        {
            // Arrange
            var command = new LogoutUserCommand();
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Cookies.Append("accessToken", "validToken", new CookieOptions());
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken(It.IsAny<string>())).Returns("userId");

            var user = new User { Id = Guid.NewGuid() };
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed());

            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserUpdateFailed, command);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenCacheFails()
        {
            // Arrange
            var command = new LogoutUserCommand();
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Cookies.Append("accessToken", "validToken", new CookieOptions());
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken(It.IsAny<string>())).Returns("userId");

            var user = new User { Id = Guid.NewGuid() };
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
            _cacheServiceMock.Setup(x => x.SetBlacklistedTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailedToSetTokenInBlackList, command);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
        }
        
      [Fact]
public async Task Handle_ReturnsSuccess_WhenLogoutIsSuccessful()
{
    // Arrange
    var command = new LogoutUserCommand();
    var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    var refreshToken = new List<DAL.Entities.Users.RefreshToken>
    {
        new DAL.Entities.Users.RefreshToken
        {
            Token = refreshTokenValue,
            Expires = DateTime.UtcNow.AddDays(2)
        }
    };

    var user = new User
    {
        Id = Guid.NewGuid(),
        RefreshTokens = refreshToken
    };

    var requestCookies = new Mock<IRequestCookieCollection>();
    requestCookies.Setup(x => x.TryGetValue("accessToken", out It.Ref<string>.IsAny!)).Returns((string key, out string value) =>
    {
        value = "validAccessToken";
        return true;
    });
    requestCookies.Setup(x => x.TryGetValue("refreshToken", out It.Ref<string>.IsAny!)).Returns((string key, out string value) =>
    {
        value = refreshTokenValue;
        return true;
    });
    requestCookies.Setup(x => x.Keys).Returns(new List<string> { "accessToken", "refreshToken" });

    var responseCookiesMock = new Mock<IResponseCookies>();
    var responseMock = new Mock<HttpResponse>();
    responseMock.Setup(r => r.Cookies).Returns(responseCookiesMock.Object);

    var requestMock = new Mock<HttpRequest>();
    requestMock.Setup(r => r.Cookies).Returns(requestCookies.Object);

    var httpContextMock = new Mock<HttpContext>();
    httpContextMock.Setup(ctx => ctx.Request).Returns(requestMock.Object);
    httpContextMock.Setup(ctx => ctx.Response).Returns(responseMock.Object);

    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);
    _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken(It.IsAny<string>())).Returns(user.Id.ToString());
    _userManagerMock.Setup(x => x.Users).Returns(new List<User> { user }.AsQueryable().BuildMockDbSet().Object);
    _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
    _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
    _cacheServiceMock.Setup(x => x.SetBlacklistedTokenAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal("User logged out successfully", result.Value);
    responseCookiesMock.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.AtLeast(2));
    _userManagerMock.Verify(x => x.UpdateAsync(It.Is<User>(u => u.RefreshTokens.Count == 0)), Times.Once);
    _cacheServiceMock.Verify(x => x.SetBlacklistedTokenAsync("validAccessToken", user.Id.ToString()), Times.Once);
}

    }
}
