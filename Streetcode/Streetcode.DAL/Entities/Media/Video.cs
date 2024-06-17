using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Entities.Media;

public class Video
{
    public int Id { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }

    public string? Url { get; set; }

    public int StreetcodeId { get; set; }

    public StreetcodeContent? Streetcode { get; set; }
}
