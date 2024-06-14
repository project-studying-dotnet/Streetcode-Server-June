using Streetcode.BLL.DTO.AdditionalContent;

namespace Streetcode.BLL.DTO.Media;

public class VideoDTO
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int StreetcodeId { get; set; }
}