using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.Services.Tokens;

public class TokenService : ITokenService
{
    private readonly UserManager<User> _userManager;
    private readonly AccessTokenConfiguration _accessTokenConfiguration;
    private readonly ILoggerService _logger;
    public TokenService(UserManager<User> userManager, AccessTokenConfiguration accessTokenConfiguration, ILoggerService logger)
    {
        _userManager = userManager;
        _accessTokenConfiguration = accessTokenConfiguration;
        _logger = logger;
    }
    
    public async Task<string> GenerateAccessToken(User user, List<Claim> claims)
    {
        if (user is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound);
            _logger.LogError(user!, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }

        if (!claims.Any())
        {
            claims = await GetUserClaimsAsync(user);
        }

        DateTime expiration = DateTime.UtcNow.AddMinutes(_accessTokenConfiguration.AccessTokenExpirationMinutes);
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_accessTokenConfiguration.SecretKey!));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken tokenGenerator = new(
            issuer: _accessTokenConfiguration.Issuer,
            audience: _accessTokenConfiguration.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: signingCredentials);

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        var token = jwtSecurityTokenHandler.WriteToken(tokenGenerator);

        return token;
    }

    public async Task<List<Claim>> GetUserClaimsAsync(User user)
    {
        if (user is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound);
            _logger.LogError(user!, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Any())
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.RolesNotFound);
            _logger.LogError(roles!, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }

        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, roles.First())
        };

        return claims;
    }

    public ClaimsPrincipal GetPrincipalFromAccessToken(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.InvalidToken);
            _logger.LogError(token!, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }
        
        JwtSecurityTokenHandler tokenHandler = new();

        if (!tokenHandler.CanReadToken(token))
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.InvalidToken);
            _logger.LogError(token, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = _accessTokenConfiguration.Audience,
            ValidateIssuer = true,
            ValidIssuer = _accessTokenConfiguration.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenConfiguration.SecretKey!)),
            ValidateLifetime = true
        };

        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        return principal;
    }
}