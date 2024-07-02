namespace Streetcode.BLL.DTO.Users
{
    public class TokenResponseDTO
    {
        public RefreshTokenDTO RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }
}
