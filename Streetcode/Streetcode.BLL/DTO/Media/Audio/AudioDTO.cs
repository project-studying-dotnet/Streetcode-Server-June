using Streetcode.BLL.DTO.AdditionalContent;

namespace Streetcode.BLL.DTO.Media.Audio;

public class AudioDTO
{
  public int Id { get; set; }
  public string Description { get; set; } = string.Empty;
  public string BlobName { get; set; } = string.Empty;
  public string Base64 { get; set; } = string.Empty;
  public string MimeType { get; set; } = string.Empty;
}