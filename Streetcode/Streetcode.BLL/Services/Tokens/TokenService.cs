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
using Microsoft.EntityFrameworkCore;

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

    public string? GetUserIdFromAccessToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;
        var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        return userIdClaim;
    }

    public async Task<TokenResponseDTO> GenerateTokens(User user)
    {
        var tokenResponse = new TokenResponseDTO();
        var userClaims = await GetUserClaimsAsync(user);
        tokenResponse.AccessToken = GenerateAccessToken(user, userClaims);
        tokenResponse.RefreshToken = GenerateRefreshToken();
       
        return tokenResponse;
    }

    public async Task RemoveExpiredRefreshToken()
    {
        var users = _userManager.Users.Include(u => u.RefreshTokens).Where(u => u.RefreshTokens.Any(t => t.Expires < DateTime.UtcNow));
        foreach (var user in users)
        {
            user.RefreshTokens.RemoveAll(t => t.Expires < DateTime.UtcNow);
            await _userManager.UpdateAsync(user);
        }
    }
    
    public RefreshTokenDTO GenerateRefreshToken()
    {
        var created = DateTime.UtcNow;
        return new RefreshTokenDTO
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Created = created,
            Expires = created.AddDays(_tokensConfiguration.RefreshTokenExpirationDays)
        };
    }

    public async Task SetRefreshToken(RefreshTokenDTO newRefreshToken, User user)
    {
        if (user == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound);
            _logger.LogError(user, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }

        if (newRefreshToken == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.InvalidToken);
            _logger.LogError(newRefreshToken, errorMsg);
            throw new ArgumentNullException(errorMsg);
        }

        var refreshToken = new RefreshToken
        {
            Token = newRefreshToken.Token,
            Created = newRefreshToken.Created,
            Expires = newRefreshToken.Expires,
            UserId = user.Id,
        };

        user.RefreshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(user);
    }
}