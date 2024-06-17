using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.DAL.Persistence.Configurations.Streetcode.TextContent
{
    internal class RelatedTermConfiguration : IEntityTypeConfiguration<RelatedTerm>
    {
        public void Configure(EntityTypeBuilder<RelatedTerm> builder)
        {
            builder.ToTable("related_terms", "streetcode");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(rt => rt.Word)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}
