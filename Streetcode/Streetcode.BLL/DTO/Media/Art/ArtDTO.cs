using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.Media.Art;

public class ArtDTO
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int ImageId { get; set; }
    public ImageDTO? Image { get; set; }
}
