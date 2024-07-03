using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Specification.Streetcode.TextContent.FactSpec.GetAll;

public class FactsFilteredByQuerySpec : Specification<Fact>
{
    public FactsFilteredByQuerySpec(string query)
    {
        Query.Include(fact => fact.Streetcode)
          .Where(x => x.Streetcode != null
          && x.Streetcode.Status == DAL.Enums.StreetcodeStatus.Published &&
                ((!string.IsNullOrEmpty(x.Title) && x.Title.Contains(query)) ||
                 (!string.IsNullOrEmpty(x.FactContent) && x.FactContent.Contains(query))));
    }
}
