namespace Streetcode.BLL.DTO.Users
{
    public class UserRegisterDTO : UserLoginDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateOnly Birthday { get; set; }       
    }
}
