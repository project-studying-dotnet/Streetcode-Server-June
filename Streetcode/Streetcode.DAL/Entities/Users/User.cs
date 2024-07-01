using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Streetcode.DAL.Enums;

namespace Streetcode.DAL.Entities.Users
{
    public class User : IdentityUser<Guid>
    {
        public DateOnly BirthDate { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
    }
}