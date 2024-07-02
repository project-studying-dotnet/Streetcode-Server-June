namespace Streetcode.BLL.DTO.Users
{
    public class UserDTO : UserRegisterDTO
    {
        public bool PhoneConfirmed { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }        
    }
}
