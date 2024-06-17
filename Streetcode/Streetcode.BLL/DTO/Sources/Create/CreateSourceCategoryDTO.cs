using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Sources.Create
{
    public class CreateSourceCategoryDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required]
        public int ImageId { get; set; }
    }
}