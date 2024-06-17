using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Sources;

namespace Streetcode.DAL.Persistence.Configurations.Sources
{
    internal class StreetcodeCategoryContentConfiguration : IEntityTypeConfiguration<StreetcodeCategoryContent>
    {
        public void Configure(EntityTypeBuilder<StreetcodeCategoryContent> builder)
        {
            builder.ToTable("streetcode_categoryContent", "sources");

            builder.Property(s => s.Text)
                .IsRequired()
                .HasMaxLength(1000);
        }
    }
}
