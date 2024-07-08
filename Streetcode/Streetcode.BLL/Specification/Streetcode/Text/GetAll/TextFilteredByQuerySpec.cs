using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Specification.Streetcode.TextSec.GetAll;

public class TextFilteredByQuerySpec : Specification<Text>
{
    public TextFilteredByQuerySpec(string query)
    {
        if(!string.IsNullOrEmpty(query))
        {
            Query.Include(x => x.Streetcode)
             .Where(x => x.Streetcode != null &&
                x.Streetcode.Status == DAL.Enums.StreetcodeStatus.Published &&
                ((!string.IsNullOrEmpty(x.Title) && x.Title.Contains(query)) ||
                 (!string.IsNullOrEmpty(x.TextContent) && x.TextContent.Contains(query))));
        }
    }
}
