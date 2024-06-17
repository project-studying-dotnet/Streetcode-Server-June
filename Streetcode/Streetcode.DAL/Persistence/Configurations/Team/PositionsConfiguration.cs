using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Team;

namespace Streetcode.DAL.Persistence.Configurations.Team
{
    internal class PositionsConfiguration : IEntityTypeConfiguration<Positions>
    {
        public void Configure(EntityTypeBuilder<Positions> builder)
        {
            builder.ToTable("positions", "team");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Position)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
