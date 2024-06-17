using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.DAL.Persistence.Configurations.Timeline
{
    internal class TimelineItemConfiguration : IEntityTypeConfiguration<TimelineItem>
    {
        public void Configure(EntityTypeBuilder<TimelineItem> builder)
        {
            builder.ToTable("timeline_items", "timeline");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Date)
                .IsRequired();

            builder.Property(t => t.DateViewPattern).IsRequired();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description).HasMaxLength(600);
        }
    }
}
