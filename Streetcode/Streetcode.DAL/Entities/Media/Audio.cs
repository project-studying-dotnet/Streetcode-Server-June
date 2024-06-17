using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Entities.Media;

public class Audio
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? BlobName { get; set; }

    public string? MimeType { get; set; }

    public string? Base64 { get; set; }

    public StreetcodeContent? Streetcode { get; set; }
}