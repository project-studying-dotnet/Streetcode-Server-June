using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Specification.Streetcode.TextContent.FactSpec.GetById;

public class FactByIdSpec : Specification<Fact>
{
    public FactByIdSpec(int factId)
    {
        Query.Where(fact => fact.Id == factId);
    }
}
