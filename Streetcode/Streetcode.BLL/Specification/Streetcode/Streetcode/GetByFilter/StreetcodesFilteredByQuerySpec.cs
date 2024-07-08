using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.Specification.Streetcode.Streetcode.GetByFilter;

public class StreetcodesFilteredByQuerySpec : Specification<StreetcodeContent>
{
     public StreetcodesFilteredByQuerySpec(string searchQuery)
     {
        Query.Where(x =>
        (x.Status == DAL.Enums.StreetcodeStatus.Published) &&
        ((!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchQuery)) ||
        (!string.IsNullOrEmpty(x.Alias) && x.Alias.Contains(searchQuery)) ||
         (!string.IsNullOrEmpty(x.Teaser) && x.Teaser.Contains(searchQuery))));
     }
}
