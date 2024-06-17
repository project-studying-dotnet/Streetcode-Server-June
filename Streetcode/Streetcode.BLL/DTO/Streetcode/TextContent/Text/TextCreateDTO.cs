using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Streetcode.BLL.Validations;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Text
{
  public class TextCreateDTO
  {
      [Required]
      [MaxLength(50)]
      public string Title { get; set; } = string.Empty;

      [Required]
      [MaxLength(15000)]
      public string TextContent { get; set; } = string.Empty;

      [MaxLength(500)]
      public string AdditionalText { get; set; } = string.Empty;

      [MaxLength(500)]
      [DefaultValue("")]
      [YoutubeUrlAttribute]
      public string VideoUrl { get; set; } = string.Empty;

      [MaxLength(200)]
      public string Author { get; set; } = string.Empty;

      [Required]
      public int StreetcodeId { get; set; }
    }
}