using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Account.LoginWithGoogle;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;
using Streetcode.BLL.Services.CookieService.Interfaces;
using Streetcode.BLL.Services.Tokens;

using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Streetcode.XUnitTest.MediatRTests.Account.LoginWithGoogle
{
    public class LoginWithGoogleHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<ICookieService> _cookieServiceMock;
        private readonly Mock<TokensConfiguration> _tokensConfigurationMock;
        private readonly Mock<LoginWithGoogleHandler> _handlerMock;

        public LoginWithGoogleHandlerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            _tokenServiceMock = new Mock<ITokenService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _cookieServiceMock = new Mock<ICookieService>();
            _tokensConfigurationMock = new Mock<TokensConfiguration>();

            _handlerMock = new Mock<LoginWithGoogleHandler>(
                _userManagerMock.Object,
                _tokenServiceMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _httpContextAccessor.Object,
                _cookieServiceMock.Object,
                _tokensConfigurationMock.Object)
            { CallBase = true }; // Allow to call base methods if not explicitly mocked
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenGoogleCredentialsAreInvalid()
        {
            // Arrange
            var command = new LoginWithGoogleCommand(new LoginWithGoogleDTO { IdToken = "invalid_token" });
            var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.InvalidToken, command);

            // Act
            var result = await _handlerMock.Object.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errorMessage, result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenMappingFails()
        {
            // Arrange
            var command = new LoginWithGoogleCommand(new LoginWithGoogleDTO { IdToken = "valid_token" });
            var googleCredentials = new Payload { Email = "test@example.com", Name = "Test User", GivenName = "Test", FamilyName = "User" };
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.CanNotMap, command);
            var user = new User { UserName = "TestUser", Email = "test@example.com" };

            _handlerMock.Setup(x => x.ValidateGoogleToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(googleCredentials);
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<UserDTO>(It.IsAny<User>())).Returns((UserDTO)null!);

            // Act
            var result = await _handlerMock.Object.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ReturnsSuccess_WhenLoginWithGoogleIsSuccessful()
        {
            // Arrange
            var command = new LoginWithGoogleCommand(new LoginWithGoogleDTO { IdToken = "valid_token" });
            var googleCredentials = new Payload { Email = "test@example.com", Name = "Test User", GivenName = "Test", FamilyName = "User" };
            var user = new User { UserName = "TestUser", Email = "test@example.com" };
            var userDto = new UserDTO { Username = "TestUser" };
            var tokens = new TokenResponseDTO { AccessToken = "access_token", RefreshToken = new RefreshTokenDTO { Token = "refresh_token" } };

            _handlerMock.Setup(x => x.ValidateGoogleToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(googleCredentials);
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<UserDTO>(user)).Returns(userDto);
            _tokenServiceMock.Setup(x => x.GenerateTokens(user)).ReturnsAsync(tokens);

            var httpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            var cookies = new Mock<IResponseCookies>();

            response.Setup(x => x.Cookies).Returns(cookies.Object);
            httpContext.Setup(x => x.Response).Returns(response.Object);
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

            _cookieServiceMock.Setup(x => x.AppendCookiesToResponseAsync(It.IsAny<HttpResponse>()));

            // Act
            var result = await _handlerMock.Object.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userDto, result.Value);
            _tokenServiceMock.Verify(x => x.GenerateTokens(user), Times.Once);
            _cookieServiceMock.Verify(x => x.AppendCookiesToResponseAsync(It.IsAny<HttpResponse>()), Times.Once);
        }
    }
}
