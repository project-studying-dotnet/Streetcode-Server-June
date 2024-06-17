using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.DAL.Persistence.Configurations.Streetcode.TextContent
{
    internal class TermConfiguration : IEntityTypeConfiguration<Term>
    {
        public void Configure(EntityTypeBuilder<Term> builder)
        {
            builder.ToTable("terms", "streetcode");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasMany(t => t.RelatedTerms)
                .WithOne(rt => rt.Term)
                .HasForeignKey(rt => rt.TermId);
        }
    }
}
