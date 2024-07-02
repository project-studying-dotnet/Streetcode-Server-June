using System.Security.Claims;

using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Moq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Account.Logout;
using Streetcode.BLL.Services.CacheService;
using Streetcode.DAL.Entities.Users;
using Streetcode.XUnitTest.MediatRTests.Account.RefreshToken;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Account.Logout
{
    public class LogoutUserHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly LogoutUserHandler _handler;

        public LogoutUserHandlerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _cacheServiceMock = new Mock<ICacheService>();
            _tokenServiceMock = new Mock<ITokenService>();

            _handler = new LogoutUserHandler(
                _userManagerMock.Object,
                _cacheServiceMock.Object,
                _loggerMock.Object,
                _httpContextAccessorMock.Object,
                _mapperMock.Object,
                _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenAccessTokenIsNotProvided()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var request = new LogoutUserCommand();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldLogoutUserSuccessfully()
        {
            // Arrange
            var userId = "563b4777-0615-4c3c-8a7d-8858412b6562";
            var userEmail = "test@example.com";
            var accessToken = "";
           
            var user = new User { Id = Guid.Parse(userId), Email = userEmail, RefreshToken = "refresh-token" };

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId)
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Cookies = new RefreshTokensHandlerTests.MockRequestCookieCollection(new Dictionary<string, string>
            {
                { "accessToken", "" }
            });

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
            _cacheServiceMock.Setup(x => x.SetBlacklistedTokenAsync(accessToken, userId)).ReturnsAsync(true);
            _mapperMock.Setup(x => x.Map<UserDTO>(It.IsAny<User>())).Returns(new UserDTO { Email = userEmail });

            var request = new LogoutUserCommand();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("User logged out successfully");

            _userManagerMock.Verify(x => x.UpdateAsync(It.Is<User>(u => u.Email == userEmail && u.RefreshToken == null)), Times.Once);
            _cacheServiceMock.Verify(x => x.SetBlacklistedTokenAsync(accessToken, userId), Times.Once);
        }
    }
}
