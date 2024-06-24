using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.Specification.Streetcode.Streetcode.GetByIndex;

public class StreetcodeByIndexInclTagsSpec : Specification<StreetcodeContent>
{
    public StreetcodeByIndexInclTagsSpec(int index)
    {
        Query.Where(sc => sc.Index == index)
            .Include(sc => sc.Tags);
    }
}
