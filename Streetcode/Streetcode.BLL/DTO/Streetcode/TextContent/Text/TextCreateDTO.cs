using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Streetcode.BLL.Validations;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Text
{
  public class TextCreateDTO
  {
      [Required]
      [MaxLength(50)]
      public string? Title { get; set; }

      [Required]
      [MaxLength(15000)]
      public string? TextContent { get; set; }

      [MaxLength(500)]
      public string? AdditionalText { get; set; }

      [MaxLength(500)]
      [DefaultValue("")]
      [YoutubeUrlAttribute]
      public string? VideoUrl { get; set; }

      [MaxLength(200)]
      public string? Author { get; set; }

      [Required]
      public int StreetcodeId { get; set; }
    }
}