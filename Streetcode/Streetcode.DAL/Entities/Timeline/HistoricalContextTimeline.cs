using System.ComponentModel.DataAnnotations;

namespace Streetcode.DAL.Entities.Timeline
{
    public class HistoricalContextTimeline
    {
        public int HistoricalContextId { get; set; }

        public int TimelineId { get; set; }

        public HistoricalContext HistoricalContext { get; set; } = new();

        public TimelineItem Timeline { get; set; } = new();
    }
}
