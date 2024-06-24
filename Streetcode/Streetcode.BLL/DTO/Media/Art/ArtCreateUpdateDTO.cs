namespace Streetcode.BLL.DTO.Media.Art
{
    public class ArtCreateUpdateDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int ImageId { get; set; }
    }
}
