using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
public class FactUpdatePositionDto 
{
    [Required]
    public int Id { get; set; }
    [Required]
    public int NewPosition { get; set; }
}