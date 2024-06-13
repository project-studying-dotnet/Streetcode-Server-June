namespace Streetcode.BLL.DTO.Streetcode.TextContent.Text
{
  public class TextCreateDTO
  {
    public string Title { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public string? AdditionalText { get; set; }
  }
}
