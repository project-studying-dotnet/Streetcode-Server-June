using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Streetcode.DAL.Entities.Timeline;

public class HistoricalContext
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public List<HistoricalContextTimeline> HistoricalContextTimelines { get; set; } = new();
}
