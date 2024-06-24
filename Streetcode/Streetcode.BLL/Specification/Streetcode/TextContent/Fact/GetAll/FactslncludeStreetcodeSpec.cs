using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Specification.Streetcode.TextContent.FactSpec.GetAll;

public class FactslncludeStreetcodeSpec : Specification<Fact>
{
    public FactslncludeStreetcodeSpec()
    {
        Query.Include(fact => fact.Streetcode)
            .Where(x => x.Streetcode != null
            && x.Streetcode.Status == DAL.Enums.StreetcodeStatus.Published);
    }
}
