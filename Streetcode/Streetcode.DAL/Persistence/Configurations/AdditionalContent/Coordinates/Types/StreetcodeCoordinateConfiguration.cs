using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Analytics;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.DAL.Persistence.Configurations.AdditionalContent.Coordinates.Types
{
    internal class StreetcodeCoordinateConfiguration : IEntityTypeConfiguration<StreetcodeCoordinate>
    {
        public void Configure(EntityTypeBuilder<StreetcodeCoordinate> builder)
        {
            builder.HasOne(sc => sc.StatisticRecord)
                .WithOne(sr => sr.StreetcodeCoordinate)
                .HasForeignKey<StatisticRecord>(sr => sr.StreetcodeCoordinateId);
        }
    }
}
