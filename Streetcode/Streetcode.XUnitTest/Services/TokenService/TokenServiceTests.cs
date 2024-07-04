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
using Streetcode.BLL.Services.Logging;
using Streetcode.BLL.Services.Tokens;
using Streetcode.DAL.Entities.Users;
using Xunit;

namespace Streetcode.XUnitTest.Services.TokenService;

public class TokenServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly TokensConfiguration _tokensConfiguration;
    private readonly BLL.Services.Tokens.TokenService _tokenService;
    private readonly ILoggerService _logger;

    public TokenServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        _tokensConfiguration = new TokensConfiguration
        {
            SecretKey = "supersecretkeythatshouldbeatleast32characters-long",
            RefreshTokenExpirationDays = 7,
            AccessTokenExpirationMinutes = 30,
            Issuer = "Streetcode",
            Audience = "StreetcodeClient"
        };
        _logger = new LoggerService(new LoggerConfiguration().CreateLogger());
        _tokenService = new BLL.Services.Tokens.TokenService(_userManagerMock.Object, _tokensConfiguration, _logger);
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnToken_WhenUserValid()
    {
        // Arrange
        var expectedUserId = Guid.NewGuid();
        var user = new User { Id = expectedUserId, UserName = "testUser", Email = "testuser@example.com" };
        _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "Admin")
        };

        // Act
        var token = _tokenService.GenerateAccessToken(user, claims);
        var actualUserId = _tokenService.GetUserIdFromAccessToken(token);

        // Assert
        Assert.NotNull(token);
        Assert.Equal(expectedUserId.ToString(), actualUserId);
    }

    [Fact]
    public void GetPrincipalFromAccessToken_ShouldReturnClaimsPrincipal_WhenTokenValid()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), UserName = "testUser", Email = "testuser@example.com" };
        var claims = new List<Claim>
          {
              new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
              new Claim(ClaimTypes.NameIdentifier, user.Email),
              new Claim(ClaimTypes.Name, user.UserName),
              new Claim(ClaimTypes.Email, user.Email),
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
        var token = jwtSecurityTokenHandler.WriteToken(tokenGenerator);

        // Act
        var principal = _tokenService.GetPrincipalFromAccessToken(token);

        // Assert
        Assert.NotNull(principal);
        Assert.IsType<ClaimsPrincipal>(principal);
        Assert.Equal(user.Email, principal.FindFirst(ClaimTypes.Email)?.Value);
    }

    [Fact]
    public async Task GetUserClaimsAsync_ShouldReturnCorrectClaims_WhenUserValid()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), UserName = "testUser", Email = "testuser@example.com" };
        _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

        // Act
        var claims = await _tokenService.GetUserClaimsAsync(user);

        // Assert
        var expectedClaims = new List<Claim>
          {
              new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.CurrentCulture)),
              new Claim(ClaimTypes.NameIdentifier, user.Email),
              new Claim(ClaimTypes.Name, user.UserName),
              new Claim(ClaimTypes.Email, user.Email),
              new Claim(ClaimTypes.Role, "Admin")
          };

        Assert.NotNull(claims);
        Assert.Equal(expectedClaims.Count, claims.Count);
    }
    [Fact]
    public async Task GenerateRefreshToken_ShouldReturnCorrectToken()
    {
        // Arrange

        // Act
        var result = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.True(result != null);
    }

    [Fact]
    public async Task SetRefreshToken_ShouldReturnFailed_WhenUserIsNull()
    {
        // Arrange
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Act
        var result = _tokenService.SetRefreshToken(refreshToken, null!);

        // Assert
        Assert.True(result.IsFaulted);
    }

    [Fact]
    public async Task SetRefreshToken_ShouldReturnFailed_WhenTokenIsNull()
    {
        // Arrange
        User user = new();

        // Act
        var result = _tokenService.SetRefreshToken(null!, user);

        // Assert
        Assert.True(result.IsFaulted);
    }

    [Fact]
    public async Task GenerateTokens_ShouldReturnCorrectResponse_WhenInputCorrectUser()
    {
        // Arrange
        TokenResponseDTO tokenResponse = new();
        var user = new User { Id = Guid.NewGuid(), UserName = "testUser", Email = "testuser@example.com" };
        _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

        // Act
        var result = _tokenService.GenerateTokens(user);

        // Assert
        Assert.True(result != null);
    }

    [Fact]
    public async Task GenerateTokens_ShouldReturnFailureResponse_WhenInputInCorrectUser()
    {
        // Arrange
        TokenResponseDTO tokenResponse = new();
        _userManagerMock.Setup(um => um.GetRolesAsync(null)).ReturnsAsync(new List<string> { "Admin" });

        // Act
        var result = _tokenService.GenerateTokens(null);

        // Assert
        Assert.True(result.IsFaulted);
    }
}
