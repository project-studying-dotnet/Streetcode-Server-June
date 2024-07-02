using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Account.RefreshToken;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Account.RefreshToken
{
    public class RefreshTokensHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly RefreshTokensHandler _handler;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IMapper> _mapperMock;

        private User _user = new User
        {
            Id = Guid.Parse("563b4777-0615-4c3c-8a7d-8858412b6562"),
            UserName = "testUser",
            Email = "testuser@example.com",
            RefreshToken = "string"
        };

        public RefreshTokensHandlerTests()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _loggerMock = new Mock<ILoggerService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _mapperMock = new Mock<IMapper>();
            _handler = new RefreshTokensHandler(_userManagerMock.Object, _loggerMock.Object, _tokenServiceMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnOkResult()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Cookies = new MockRequestCookieCollection(new Dictionary<string, string>
            {
                { "accessToken", "validAccessToken" },
                { "refreshToken", "string" }
            });
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Id.ToString())
            }));

            _tokenServiceMock.Setup(x => x.GetPrincipalFromAccessToken(It.IsAny<string>())).Returns(claimsPrincipal);
            _userManagerMock.Setup(x => x.Users).Returns((new List<User> { _user }).AsQueryable());
            _tokenServiceMock.Setup(x => x.GenerateAndSetTokensAsync(_user, httpContext.Response)).Returns(Task.CompletedTask);

            var command = new RefreshTokensCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _tokenServiceMock.Verify(x => x.GenerateAndSetTokensAsync(_user, httpContext.Response), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyTokenResponse_ShouldReturnFailed()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var command = new RefreshTokensCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorMessages.EntityNotFound, result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_MissingAccessToken_ShouldReturnFailed()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var command = new RefreshTokensCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorMessages.AccessTokenNotFound, result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_MissingRefreshToken_ShouldReturnFailed()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Cookies = new MockRequestCookieCollection(new Dictionary<string, string>
            {
                { "accessToken", "validAccessToken" }
            });
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var command = new RefreshTokensCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorMessages.RefreshTokenNotFound, result.Errors.First().Message);
        }

        // MockRequestCookieCollection implementation
        public class MockRequestCookieCollection : IRequestCookieCollection
        {
            private readonly Dictionary<string, string> _cookies;

            public MockRequestCookieCollection(Dictionary<string, string> cookies)
            {
                _cookies = cookies;
            }

            public string this[string key] => _cookies.ContainsKey(key) ? _cookies[key] : null;

            public int Count => _cookies.Count;

            public ICollection<string> Keys => _cookies.Keys;

            public bool ContainsKey(string key) => _cookies.ContainsKey(key);

            public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _cookies.GetEnumerator();

            public bool TryGetValue(string key, out string value) => _cookies.TryGetValue(key, out value);

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _cookies.GetEnumerator();
        }
    }
}
