using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.DAL.Persistence.Configurations.Timeline
{
    internal class HistoricalContextConfiguration : IEntityTypeConfiguration<HistoricalContext>
    {
        public void Configure(EntityTypeBuilder<HistoricalContext> builder)
        {
            builder.ToTable("historical_contexts", "timeline");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id).ValueGeneratedOnAdd();

            builder.Property(h => h.Title)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
