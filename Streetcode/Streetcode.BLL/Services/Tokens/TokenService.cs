using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;
using FluentResults;
using Org.BouncyCastle.Asn1.Ocsp;
using Newtonsoft.Json.Linq;

namespace Streetcode.BLL.Services.Tokens;

public class TokenService : ITokenService
{
    private readonly UserManager<User> _userManager;
    private readonly TokensConfiguration _tokensConfiguration;
    private readonly ILoggerService _logger;

    public TokenService(UserManager<User> userManager, TokensConfiguration tokensConfiguration, ILoggerService logger)
    {
        _userManager = userManager;
        _tokensConfiguration = tokensConfiguration;
        _logger = logger;
    }
    
    public string GenerateAccessToken(User user, List<Claim> claims)
    {
        if (user is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, user);
            _logger.LogError(user!, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }

        if (!claims.Any())
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.ClaimsNotExist, claims);
            _logger.LogError(user!, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }

        var expiration = DateTime.UtcNow.AddMinutes(_tokensConfiguration.AccessTokenExpirationMinutes);
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_tokensConfiguration.SecretKey!));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken tokenGenerator = new(
            issuer: _tokensConfiguration.Issuer,
            audience: _tokensConfiguration.Audience,
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
            ValidAudience = _tokensConfiguration.Audience,
            ValidateIssuer = true,
            ValidIssuer = _tokensConfiguration.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokensConfiguration.SecretKey!)),
            ValidateLifetime = true
        };

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        return principal;
    }

    public RefreshTokenDTO GenerateRefreshToken()
    {
        var refreshToken = new RefreshTokenDTO
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(_tokensConfiguration.RefreshTokenExpirationDays)
        };

        return refreshToken;
    }

    public string? GetUserIdFromAccessToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;
        var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        return userIdClaim;
    }
    
    public async Task SetRefreshToken(RefreshTokenDTO newRefreshToken, User user)
    {
        if(user == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound);
            _logger.LogError(user, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }

        if(newRefreshToken == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.InvalidToken);
            _logger.LogError(newRefreshToken, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }

        user.RefreshToken = newRefreshToken.Token;
        user.Created = newRefreshToken.Created;
        user.Expires = newRefreshToken.Expires;
        await _userManager.UpdateAsync(user);
    }

    public async Task<TokenResponseDTO> GenerateTokens(User user)
    {
        if (user == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound);
            _logger.LogError(user, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }

        var tokenResponse = new TokenResponseDTO();
        var userClaims = await GetUserClaimsAsync(user);
        tokenResponse.AccessToken = GenerateAccessToken(user, userClaims);
        tokenResponse.RefreshToken = GenerateRefreshToken();
        await SetRefreshToken(tokenResponse.RefreshToken, user);

        return tokenResponse;
    }

    public async Task RemoveExpiredRefreshToken()
    {
        var users = _userManager.Users.Where(u => u.Expires < DateTime.UtcNow);
        foreach (var user in users)
        {
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }
    }
    
    public async Task GenerateAndSetTokensAsync(User user, HttpResponse response)
    {
        var tokens = await GenerateTokens(user);

        response.Cookies.Append("accessToken", tokens.AccessToken, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddMinutes(_tokensConfiguration.AccessTokenExpirationMinutes),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        response.Cookies.Append("refreshToken", tokens.RefreshToken.Token, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(_tokensConfiguration.RefreshTokenExpirationDays),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
    }
}