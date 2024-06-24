using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.Specification.Streetcode.Streetcode.GetAll;

public class StreetcodeApplyPaginationSpec : Specification<StreetcodeContent>
{
    public StreetcodeApplyPaginationSpec(int amount, int page)
    {
        Query.Skip((page - 1) * amount)
                 .Take(amount);
    }
}
