using Ardalis.Specification;
using Streetcode.DAL.Entities.Timeline;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Streetcode.BLL.Specification.TimeLine;

public class TimeLinesIncludePublishStreetcodeSpec : Specification<TimelineItem>
{
    public TimeLinesIncludePublishStreetcodeSpec(string query)
    {
        Query.Include(tm => tm.Streetcode)
            .Where(tm => tm.Streetcode != null &&
            tm.Streetcode.Status == DAL.Enums.StreetcodeStatus.Published &&
                ((!string.IsNullOrEmpty(tm.Title) && tm.Title.Contains(query)) ||
                 (!string.IsNullOrEmpty(tm.Description) && tm.Description.Contains(query))));
    }
}
