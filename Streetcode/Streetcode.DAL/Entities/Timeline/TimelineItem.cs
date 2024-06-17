using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;

namespace Streetcode.DAL.Entities.Timeline;

public class TimelineItem
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public DateViewPattern DateViewPattern { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int StreetcodeId { get; set; }

    public StreetcodeContent? Streetcode { get; set; }

    public List<HistoricalContextTimeline> HistoricalContextTimelines { get; set; } = new ();
}
