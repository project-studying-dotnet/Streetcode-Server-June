using System.ComponentModel.DataAnnotations;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Entities.Toponyms
{
    public class StreetcodeToponym
    {
        public int StreetcodeId { get; set; }

        public int ToponymId { get; set; }

        public StreetcodeContent? Streetcode { get; set; }

        public Toponym? Toponym { get; set; }
    }
}
