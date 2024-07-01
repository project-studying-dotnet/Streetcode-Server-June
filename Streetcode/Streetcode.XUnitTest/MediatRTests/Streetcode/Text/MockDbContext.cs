using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Streetcode;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;
using RelatedTermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;
using TermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Text;

public class MockDbContext : DbContext
{
    public MockDbContext(DbContextOptions<MockDbContext> options) : base(options)
    {
    }

    public DbSet<TextEntity> TextEntities { get; set; }
    public DbSet<RelatedTermEntity> RelatedTermEntityEntities { get; set; }
    public DbSet<TermEntity> TermEntityEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StreetcodeContent>()
            .Ignore(c => c.Observers)
            .Ignore(c => c.Targets)
            .Ignore(c => c.Partners)
            .Ignore(c => c.StreetcodeArts)
            .Ignore(c => c.StreetcodeCategoryContents)
            .Ignore(c => c.StatisticRecords)
            .Ignore(c => c.Coordinates)
            .Ignore(c => c.Toponyms)
            .Ignore(c => c.Images)
            .Ignore(c => c.StreetcodeTagIndices)
            .Ignore(c => c.Tags)
            .Ignore(c => c.Subtitles)
            .Ignore(c => c.Facts)
            .Ignore(c => c.Videos)
            .Ignore(c => c.SourceLinkCategories)
            .Ignore(c => c.TimelineItems);
    }
}