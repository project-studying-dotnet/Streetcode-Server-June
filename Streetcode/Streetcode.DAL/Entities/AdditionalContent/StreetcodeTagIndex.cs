using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Entities.AdditionalContent
{
    public class StreetcodeTagIndex
    {
        public int StreetcodeId { get; set; }

        public int TagId { get; set; }

        public bool IsVisible { get; set; }

        public int Index { get; set; }

        public StreetcodeContent? Streetcode { get; set; }

        public Tag? Tag { get; set; }
    }
}
