namespace Streetcode.BLL.DTO.Users
{
    public class RefreshTokenResponce
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireAt { get; set; }
    }
}
