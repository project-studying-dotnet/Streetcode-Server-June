namespace Streetcode.BLL.DTO.Users
{
    public class LoginResultDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
