using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Account.RefreshToken;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.XUnitTest.MediatRTests.Account.RefreshToken
{
    public class RefreshTokensHandlerTests
    {
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly RefreshTokensHandler _handler;

        public RefreshTokensHandlerTests()
        {
            _loggerMock = new Mock<ILoggerService>();
            _tokenServiceMock = new Mock<ITokenService>();
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _handler = new RefreshTokensHandler(_userManagerMock.Object, _loggerMock.Object, _tokenServiceMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenRefreshTokenNotFoundInCookies()
        {
            // Arrange
            var command = new RefreshTokensCommand();
            var httpContext = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.RefreshTokenNotFound, command);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
            _loggerMock.Verify(logger => logger.LogError(It.IsAny<object>(), errorMsg), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenUserWithRefreshTokenNotFound()
        {
            // Arrange
            var command = new RefreshTokensCommand();
            var httpContext = new DefaultHttpContext();
            var tokens = new TokenResponseDTO { AccessToken = "access_token", RefreshToken = new RefreshTokenDTO { Token = "refresh_token" } };
            _tokenServiceMock.Setup(x => x.GenerateAndSetTokensAsync(It.IsAny<User>(), It.IsAny<HttpResponse>()))
                .Callback<User, HttpResponse>((_, response) =>
                {
                    response.Cookies.Append("accessToken", tokens.AccessToken, new CookieOptions());
                    response.Cookies.Append("refreshToken", tokens.RefreshToken.Token, new CookieOptions());
                });
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            _userManagerMock.Setup(x => x.Users).Returns(Enumerable.Empty<User>().AsQueryable());

            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.RefreshTokenNotFound, command);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
            _loggerMock.Verify(logger => logger.LogError(It.IsAny<object>(), errorMsg), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsSuccess_WhenTokensRefreshedSuccessfully()
        {
            // Arrange
            var command = new RefreshTokensCommand();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string> { { "refreshToken", "validRefreshToken" } });

            var tokens = new TokenResponseDTO { AccessToken = "access_token", RefreshToken = new RefreshTokenDTO { Token = "refresh_token" } };

            var user = new User { UserName = "TestUser", RefreshToken = "validRefreshToken" };
            _userManagerMock.Setup(x => x.Users).Returns(new List<User> { user }.AsQueryable());

            var responseCookiesMock = new Mock<IResponseCookies>();
            var responseMock = new Mock<HttpResponse>();
            responseMock.Setup(r => r.Cookies).Returns(responseCookiesMock.Object);

            httpContext.Response = responseMock.Object;
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            _tokenServiceMock.Setup(x => x.GenerateAndSetTokensAsync(It.IsAny<User>(), It.IsAny<HttpResponse>()))
                .Callback<User, HttpResponse>((_, response) =>
                {
                    response.Cookies.Append("accessToken", tokens.AccessToken, new CookieOptions());
                    response.Cookies.Append("refreshToken", tokens.RefreshToken.Token, new CookieOptions());
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Tokens refreshed successfully!", result.Value);
            _tokenServiceMock.Verify(x => x.GenerateAndSetTokensAsync(user, httpContext.Response), Times.Once);
            responseCookiesMock.Verify(x => x.Append("accessToken", tokens.AccessToken, It.IsAny<CookieOptions>()), Times.Once);
            responseCookiesMock.Verify(x => x.Append("refreshToken", tokens.RefreshToken.Token, It.IsAny<CookieOptions>()), Times.Once);
        }

    }
}
