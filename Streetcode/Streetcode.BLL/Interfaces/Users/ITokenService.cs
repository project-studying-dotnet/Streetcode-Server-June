using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Streetcode.BLL.DTO.Users;
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
        Task GenerateAndSetTokensAsync(User user, HttpResponse httpContext);
        string? GetUserIdFromAccessToken(string accessToken);
    }
}
