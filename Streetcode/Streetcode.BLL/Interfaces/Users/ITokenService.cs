using System.Security.Claims;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.Interfaces.Users
{
    public interface ITokenService
    {
        Task<(string Token, DateTime Expiration)> GenerateAccessToken(User user, List<Claim> claims);
        Task<List<Claim>> GetUserClaimsAsync(User user);
        ClaimsPrincipal GetPrincipalFromAccessToken(string? token);
    }
}
