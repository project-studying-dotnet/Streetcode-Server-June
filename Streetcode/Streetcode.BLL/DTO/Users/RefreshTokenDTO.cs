namespace Streetcode.BLL.DTO.Users
{
    public class RefreshTokenDTO
    {
        public string Token { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Expires { get; set; }
    }
}
