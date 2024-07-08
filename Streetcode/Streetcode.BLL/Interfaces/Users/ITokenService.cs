using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Services.Tokens;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.Interfaces.Users
{
    public interface ITokenService
    {        
        string GenerateAccessToken(User user, List<Claim> claims);
        Task<List<Claim>> GetUserClaimsAsync(User user);
        ClaimsPrincipal GetPrincipalFromAccessToken(string? token);
        RefreshTokenDTO GenerateRefreshToken();
        Task SetRefreshToken(RefreshTokenDTO newRefreshToken, User user);
        Task<TokenResponseDTO> GenerateTokens(User user);        
        string? GetUserIdFromAccessToken(string accessToken);
    }
}
