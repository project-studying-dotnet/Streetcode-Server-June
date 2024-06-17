using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.DTO.Media.Images;

public class ImageDTO
{
    public int Id { get; set; }

    public string BlobName { get; set; } = string.Empty;
    public string Base64 { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public ImageDetailsDto ImageDetails { get; set; } = new();
}
