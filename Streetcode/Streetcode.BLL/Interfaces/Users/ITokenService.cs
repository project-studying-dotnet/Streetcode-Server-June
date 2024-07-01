using System.Security.Claims;
using Streetcode.BLL.DTO.Users;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.Interfaces.Users
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user, List<Claim> claims);
        Task<List<Claim>> GetUserClaimsAsync(User user);
        ClaimsPrincipal GetPrincipalFromAccessToken(string? token);
        RefreshTokenDTO GenerateRefreshToken();
        Task SetRefreshToken(RefreshTokenDTO newRefreshToken, User user);
        Task<TokenResponseDTO> GenerateTokens(User user);
    }
}
