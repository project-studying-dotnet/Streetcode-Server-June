namespace Streetcode.DAL.Entities.Users
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
