using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Specification.Streetcode.TextSpec.GetAll;

public class TextsIncludeStreetcodeSpec : Specification<Text>
{
    public TextsIncludeStreetcodeSpec()
    {
        Query.Include(x => x.Streetcode)
              .Where(x => x.Streetcode != null &&
              x.Streetcode.Status == DAL.Enums.StreetcodeStatus.Published);
    }
}