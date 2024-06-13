using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Entities.News
{
    [Table("news", Schema = "news")]
    [Index(nameof(URL), IsUnique = true)]
    public class News
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Text { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string URL { get; set; } = string.Empty;
        public int? ImageId { get; set; }
        public Image? Image { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
    }
}
