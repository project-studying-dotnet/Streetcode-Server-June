using Streetcode.BLL.DTO.AdditionalContent;

namespace Streetcode.BLL.DTO.Streetcode.RelatedFigure;

public class RelatedFigureDTO
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Url { get; set; } = string.Empty;
  public string Alias { get; set; } = string.Empty;
  public int ImageId { get; set; }
  public IEnumerable<TagDTO>? Tags { get; set; }
}
