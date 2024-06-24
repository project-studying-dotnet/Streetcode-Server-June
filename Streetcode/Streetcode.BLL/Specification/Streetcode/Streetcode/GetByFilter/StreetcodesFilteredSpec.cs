using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.Specification.Streetcode.Streetcode.GetByFilter;

public class StreetcodesFilteredSpec : Specification<StreetcodeContent>
{
    public StreetcodesFilteredSpec(string searchQuery)
    {
        if (!string.IsNullOrEmpty(searchQuery))
        {
            Query.Where(x =>
            x.Status == DAL.Enums.StreetcodeStatus.Published &&
            ((x.Title != null && x.Title.Contains(searchQuery)) ||
            (x.Alias != null && x.Alias.Contains(searchQuery)) ||
            (x.Teaser != null && x.Teaser.Contains(searchQuery))));
        }
    }
}
