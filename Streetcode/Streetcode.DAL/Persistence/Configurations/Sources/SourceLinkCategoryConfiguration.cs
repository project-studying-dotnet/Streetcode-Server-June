using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Sources;

namespace Streetcode.DAL.Persistence.Configurations.Sources
{
    internal class SourceLinkCategoryConfiguration : IEntityTypeConfiguration<SourceLinkCategory>
    {
        public void Configure(EntityTypeBuilder<SourceLinkCategory> builder)
        {
            builder.ToTable("source_link_categories", "sources");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.HasMany(d => d.StreetcodeCategoryContents)
                .WithOne(p => p.SourceLinkCategory)
                .HasForeignKey(d => d.SourceLinkCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
