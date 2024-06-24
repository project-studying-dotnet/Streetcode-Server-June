using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Specification.Streetcode.TextContent.FactSpec.GetByStreetcode;

public class FactsByStreetcodeIdSpec : Specification<Fact>
{
    public FactsByStreetcodeIdSpec(int streetcodeId)
    {
        Query.Where(fact => fact.StreetcodeId == streetcodeId);
    }
}
