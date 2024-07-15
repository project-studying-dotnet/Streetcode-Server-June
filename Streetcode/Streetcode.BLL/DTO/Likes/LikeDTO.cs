using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.DTO.Likes
{
    public class LikeDTO
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public UserDTO UserDTO { get; set; }
        public int streetcodeId { get; set; }
        public StreetcodeDTO StreetcodeDTO { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    }
}
