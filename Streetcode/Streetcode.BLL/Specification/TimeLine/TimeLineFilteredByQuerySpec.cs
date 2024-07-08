using Ardalis.Specification;
using Org.BouncyCastle.Asn1.X509;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.BLL.Specification.TimeLine;

public class TimeLineFilteredByQuerySpec : Specification<TimelineItem>
{
    public TimeLineFilteredByQuerySpec(string query)
    {
        Query.Include(tm => tm.Streetcode)
            .Where(tm => tm.Streetcode != null &&
            tm.Streetcode.Status == DAL.Enums.StreetcodeStatus.Published);
    }
}
