using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Enums;

namespace Streetcode.DAL.Entities.Users
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public UserRole Role { get; set; }
    }
}
