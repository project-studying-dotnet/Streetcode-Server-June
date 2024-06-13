using System.ComponentModel.DataAnnotations;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Users
{
    public class UserDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string Surname { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MaxLength(20)]
        public string Login { get; set; } = string.Empty;
        [Required]
        [MaxLength(20)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public UserRole Role { get; set; }
    }
}
