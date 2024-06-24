using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.Specification.Streetcode.Streetcode.GetAllCatalog;

public class StreetcodesInclTagsImagesSpec : Specification<StreetcodeContent>
{
    public StreetcodesInclTagsImagesSpec()
    {
        Query.Where(sc => sc.Status == DAL.Enums.StreetcodeStatus.Published)
             .Include(sc => sc.Tags)
             .Include(sc => sc.Images);
    }
}
