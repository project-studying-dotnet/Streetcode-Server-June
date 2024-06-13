namespace Streetcode.BLL.DTO.Streetcode
{
    public class StreetcodeFilterResultDTO
    {
        public int StreetcodeId { get; set; }
        public string StreetcodeTransliterationUrl { get; set; } = string.Empty;
        public int StreetcodeIndex { get; set; }
        public string BlockName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
    }
}
