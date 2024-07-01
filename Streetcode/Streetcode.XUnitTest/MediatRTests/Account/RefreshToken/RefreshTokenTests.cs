using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Serilog;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Account.RefreshTokens;
using Streetcode.BLL.Services.Logging;
using Streetcode.BLL.Services.Tokens;
using Streetcode.DAL.Entities.Users;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Account.RefreshToken
{
    public class RefreshTokensHandlerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly TokensConfiguration _tokensConfiguration;
        private readonly ILoggerService _logger;
        private readonly RefreshTokensHandler _handler;
        private User _user = new User { Id = Guid.NewGuid(), UserName = "testUser", Email = "testuser@example.com", RefreshToken = "string" };

        public RefreshTokensHandlerTests()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _tokensConfiguration = new TokensConfiguration
            {
                SecretKey = "supersecretkeythatshouldbeatleast32characters-long",
                AccessTokenExpirationMinutes = 30,
                Issuer = "Streetcode",
                Audience = "StreetcodeClient"
            };
            _logger = new LoggerService(new LoggerConfiguration().CreateLogger());
            _handler = new RefreshTokensHandler(_userManagerMock.Object, _logger, _tokensConfiguration);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnOkResult()
        {
            // Arrange
            var token = GetAccessTokenForGetPrincipal();
            var tokenResponse = new TokenResponseDTO
            {
                AccessToken = token,
                RefreshToken = new RefreshTokenDTO
                {
                    Token = "refToken"
                }
            };
            List<string> role = new() { "user" };
            var command = new RefreshTokensCommand(tokenResponse);
            _userManagerMock.Setup(obj => obj.FindByEmailAsync("testuser@example.com")).ReturnsAsync(_user);
            _userManagerMock.Setup(obj => obj.GetRolesAsync(_user)).ReturnsAsync(role);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_EmptyTokenResponse_ShouldReturnFailed()
        {
            // Arrange
            var token = GetAccessTokenForGetPrincipal();
            List<string> role = new() { "user" };
            var command = new RefreshTokensCommand(null);
            _userManagerMock.Setup(obj => obj.FindByEmailAsync("testuser@example.com")).ReturnsAsync(_user);
            _userManagerMock.Setup(obj => obj.GetRolesAsync(_user)).ReturnsAsync(role);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
        }

        private string GetAccessTokenForGetPrincipal()
        {
            var claims = new List<Claim>
          {
              new Claim(JwtRegisteredClaimNames.Sub, _user.Id.ToString()),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
              new Claim(ClaimTypes.NameIdentifier, _user.Email),
              new Claim(ClaimTypes.Name, _user.UserName),
              new Claim(ClaimTypes.Email, _user.Email),
              new Claim(ClaimTypes.Role, "Admin")
          };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokensConfiguration.SecretKey!));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenGenerator = new JwtSecurityToken(
                issuer: _tokensConfiguration.Issuer,
                audience: _tokensConfiguration.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_tokensConfiguration.AccessTokenExpirationMinutes),
                signingCredentials: signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            return jwtSecurityTokenHandler.WriteToken(tokenGenerator);
        }
    }
}
