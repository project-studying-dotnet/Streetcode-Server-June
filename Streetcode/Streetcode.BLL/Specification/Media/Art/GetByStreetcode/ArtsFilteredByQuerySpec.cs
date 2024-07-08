using Ardalis.Specification;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.Specification.Media.ArtSpec.GetByStreetcode;

public class ArtsFilteredByQuerySpec : Specification<Art>
{
    public ArtsFilteredByQuerySpec(string query)
    {
        Query.Include(x => x.StreetcodeArts)
             .Where(x => x.StreetcodeArts.Any(art => art.Streetcode != null
             && art.Streetcode.Status == DAL.Enums.StreetcodeStatus.Published)
             && (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(query)));
    }
}
