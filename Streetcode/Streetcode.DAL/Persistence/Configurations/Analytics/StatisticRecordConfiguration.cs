using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Analytics;

namespace Streetcode.DAL.Persistence.Configurations.Analytics
{
    internal class StatisticRecordConfiguration : IEntityTypeConfiguration<StatisticRecord>
    {
        public void Configure(EntityTypeBuilder<StatisticRecord> builder)
        {
            builder.ToTable("qr_coordinates", "coordinates");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            builder.Property(s => s.Address).HasMaxLength(150);

            builder
              .HasOne(x => x.StreetcodeCoordinate)
              .WithOne(x => x.StatisticRecord)
              .HasForeignKey<StatisticRecord>(x => x.StreetcodeCoordinateId);
        }
    }
}
