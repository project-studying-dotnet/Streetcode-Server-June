using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Media.Images
{
    public class ImageDetailsDto
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Alt { get; set; } = string.Empty;

        public int ImageId { get; set; }
    }
}
