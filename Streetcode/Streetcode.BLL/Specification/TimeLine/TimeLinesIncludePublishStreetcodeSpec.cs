using Ardalis.Specification;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.BLL.Specification.TimeLine;

public class TimeLinesIncludePublishStreetcodeSpec : Specification<TimelineItem>
{
    public TimeLinesIncludePublishStreetcodeSpec()
    {
        Query.Include(tm => tm.Streetcode)
            .Where(tm => tm.Streetcode != null &&
            tm.Streetcode.Status == DAL.Enums.StreetcodeStatus.Published);
    }
}
