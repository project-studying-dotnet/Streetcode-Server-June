using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.Specification.Streetcode.Streetcode.GetByFilter;

public class StreetcodesFilteredByStatusSpec : Specification<StreetcodeContent>
{
    public StreetcodesFilteredByStatusSpec(string filter)
    {
        var filterParams = filter.Split(':');
        if (filterParams.Length == 2)
        {
            var filterValue = filterParams[1];

            if (Enum.TryParse(filterValue, out StreetcodeStatus status))
            {
                Query.Where(s => s.Status == status);
            }
        }
    }
}
