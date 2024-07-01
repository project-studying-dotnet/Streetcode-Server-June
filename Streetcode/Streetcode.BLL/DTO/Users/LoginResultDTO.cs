namespace Streetcode.BLL.DTO.Users
{
    public class LoginResultDTO
    {
        public UserDTO User { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public RefreshTokenDTO RefreshToken { get; set; } 
    }
}
