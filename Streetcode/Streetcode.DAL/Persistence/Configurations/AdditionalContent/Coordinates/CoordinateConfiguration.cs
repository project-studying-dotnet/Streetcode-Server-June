using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates;

namespace Streetcode.DAL.Persistence.Configurations.AdditionalContent.Coordinates
{
    internal class CoordinateConfiguration : IEntityTypeConfiguration<Coordinate>
    {
        public void Configure(EntityTypeBuilder<Coordinate> builder)
        {
            builder.ToTable("coordinates", "add_content");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Latitude)
                .IsRequired()
                .HasColumnType("decimal(18,4)");

            builder.Property(c => c.Longtitude)
                .IsRequired()
                .HasColumnType("decimal(18,4)");

            builder
                .HasDiscriminator<string>("CoordinateType")
                .HasValue<Coordinate>("coordinate_base")
                .HasValue<StreetcodeCoordinate>("coordinate_streetcode")
                .HasValue<ToponymCoordinate>("coordinate_toponym");
        }
    }
}
