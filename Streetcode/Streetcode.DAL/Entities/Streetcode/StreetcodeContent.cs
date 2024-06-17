using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Analytics;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Entities.Transactions;
using Streetcode.DAL.Enums;

namespace Streetcode.DAL.Entities.Streetcode;

public class StreetcodeContent
{
    public int Id { get; set; }

    public int Index { get; set; }

    public string? Teaser { get; set; }

    public string? DateString { get; set; }

    public string? Alias { get; set; }

    public StreetcodeStatus Status { get; set; }

    public string? Title { get; set; }

    public string? TransliterationUrl { get; set; }

    public int ViewCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime EventStartOrPersonBirthDate { get; set; }

    public DateTime? EventEndOrPersonDeathDate { get; set; }

    public int? AudioId { get; set; }

    public List<Text> Texts { get; set; } = new();

    public Audio? Audio { get; set; }

    public List<StatisticRecord> StatisticRecords { get; set; } = new();

    public List<StreetcodeCoordinate> Coordinates { get; set; } = new();

    public TransactionLink? TransactionLink { get; set; }

    public List<Toponym> Toponyms { get; set; } = new ();

    public List<Image> Images { get; set; } = new ();

    public List<StreetcodeTagIndex> StreetcodeTagIndices { get; set; } = new (); // typo here Indices

    public List<Tag> Tags { get; set; } = new();

    public List<Subtitle> Subtitles { get; set; } = new ();

    public List<Fact> Facts { get; set; } = new ();

    public List<Video> Videos { get; set; } = new ();

    public List<SourceLinkCategory> SourceLinkCategories { get; set; } = new ();

    public List<TimelineItem> TimelineItems { get; set; } = new ();

    public List<RelatedFigure> Observers { get; set; } = new ();

    public List<RelatedFigure> Targets { get; set; } = new ();

    public List<Partner> Partners { get; set; } = new ();

    public List<StreetcodeArt> StreetcodeArts { get; set; } = new ();

    public List<StreetcodeCategoryContent> StreetcodeCategoryContents { get; set; } = new();
}
