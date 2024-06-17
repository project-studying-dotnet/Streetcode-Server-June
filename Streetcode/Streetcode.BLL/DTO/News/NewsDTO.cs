using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.News
{
    public class NewsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int? ImageId { get; set; }
        public string URL { get; set; } = string.Empty;
        public ImageDTO Image { get; set; } = new();
        public DateTime CreationDate { get; set; }
    }
}
