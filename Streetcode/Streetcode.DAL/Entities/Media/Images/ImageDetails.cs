using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Streetcode.DAL.Entities.Media.Images
{
    public class ImageDetails
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Alt { get; set; }

        public int ImageId { get; set; }

        public Image? Image { get; set; }
    }
}
