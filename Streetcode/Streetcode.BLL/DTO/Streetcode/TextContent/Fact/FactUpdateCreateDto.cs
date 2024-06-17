using System.ComponentModel.DataAnnotations;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Fact
{
    public class FactUpdateCreateDto : FactDto
    {
        [MaxLength(200)]
        public string ImageDescription { get; set; } = string.Empty;
    }
}