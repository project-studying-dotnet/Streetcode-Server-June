namespace Streetcode.BLL.DTO.AdditionalContent.Subtitles;

public class SubtitleDTO
{
    public int Id { get; set; }
    public string SubtitleText { get; set; } = string.Empty;
    public int StreetcodeId { get; set; }
}
