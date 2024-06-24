using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.Specification.Streetcode.Streetcode.GetAll;

public class StreetcodesFindWithMatchTitleSpec : Specification<StreetcodeContent>
{
    public StreetcodesFindWithMatchTitleSpec(string title)
    {
        var lowerCaseTitle = title.ToLower();
        Query.Where(s => (!string.IsNullOrEmpty(s.Title) && (s.Title.ToLower().Contains(lowerCaseTitle) ||
        s.Index.ToString() == title)));
    }
}
