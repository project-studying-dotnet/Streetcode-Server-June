using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.Streetcode.RelatedFigure;

public class RelatedFigureDTO
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Url { get; set; } = string.Empty;
  public string Alias { get; set; } = string.Empty;
  public IEnumerable<ImageDTO>? Images { get; set; }
  public IEnumerable<TagDTO>? Tags { get; set; }
  public int? CurrentStreetcodeId { get; set; }
}
