using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.DAL.Entities.Likes
{
    public class Like
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public int streetcodeId { get; set; }
        public StreetcodeContent Streetcode { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    }
}
