using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Streetcode.DAL.Entities.Media.Images
{
    [Table("image_details", Schema = "media")]
    public class ImageDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Alt { get; set; } = string.Empty;

        [Required]
        public int ImageId { get; set; }

        public Image? Image { get; set; }
    }
}
