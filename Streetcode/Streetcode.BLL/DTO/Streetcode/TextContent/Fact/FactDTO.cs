using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

public class FactDto
{
    public int Id { get; set; }
    [Required]
    [MaxLength(68)]
    public string Title { get; set; } = string.Empty;
    [Required]
    public int ImageId { get; set; }
    [Required]
    [MaxLength(600)]
    public string FactContent { get; set; } = string.Empty;
    [Required]
    public int StreetcodeId { get; set; }
    public int Position { get; set; }
}
