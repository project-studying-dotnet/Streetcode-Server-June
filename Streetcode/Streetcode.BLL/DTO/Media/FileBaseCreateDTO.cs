namespace Streetcode.BLL.DTO.Media;

public class FileBaseCreateDTO
{
    public string Title { get; set; } = string.Empty;
    public string BaseFormat { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
}
