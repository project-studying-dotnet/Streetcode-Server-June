using Ardalis.Specification;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.Specification.Media.ArtSpec.GetByStreetcode;

public class ArtByPublStreetcodesIncludeImageSpec : Specification<Art>
{
    public ArtByPublStreetcodesIncludeImageSpec()
    {
        Query.Include(x => x.StreetcodeArts)
            .Where(x => x.StreetcodeArts.Any(art => art.Streetcode != null
            && art.Streetcode.Status == DAL.Enums.StreetcodeStatus.Published))
            .Include(x => x.Image);
    }
}
