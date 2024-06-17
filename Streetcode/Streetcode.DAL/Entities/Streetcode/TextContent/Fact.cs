using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Entities.Streetcode.TextContent;

public class Fact
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? FactContent { get; set; }

    public int? Position { get; set; }

    public int? ImageId { get; set; }

    public Image? Image { get; set; }

    public int StreetcodeId { get; set; }

    public StreetcodeContent? Streetcode { get; set; }
}
