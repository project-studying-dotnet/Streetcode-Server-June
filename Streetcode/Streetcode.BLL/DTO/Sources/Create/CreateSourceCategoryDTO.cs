using System.ComponentModel.DataAnnotations;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Entities.Streetcode;

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
