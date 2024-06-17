using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.DAL.Persistence.Configurations.Streetcode.TextContent
{
    internal class FactConfiguration : IEntityTypeConfiguration<Fact>
    {
        public void Configure(EntityTypeBuilder<Fact> builder)
        {
            builder.ToTable("facts", "streetcode");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id).ValueGeneratedOnAdd();

            builder.Property(f => f.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.FactContent)
                .IsRequired()
                .HasMaxLength(600);
        }
    }
}
