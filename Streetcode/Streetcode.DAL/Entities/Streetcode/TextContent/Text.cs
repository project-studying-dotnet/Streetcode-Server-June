using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Streetcode.DAL.Entities.Streetcode.TextContent;

public class Text
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? TextContent { get; set; }

    public string? AdditionalText { get; set; }

    public string? VideoUrl { get; set; }

    public string? Author { get; set; }
  
    public int StreetcodeId { get; set; }

    public StreetcodeContent? Streetcode { get; set; }
}