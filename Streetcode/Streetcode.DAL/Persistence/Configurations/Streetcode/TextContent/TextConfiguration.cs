using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.DAL.Persistence.Configurations.Streetcode.TextContent
{
    internal class TextConfiguration : IEntityTypeConfiguration<Text>
    {
        public void Configure(EntityTypeBuilder<Text> builder)
        {
            builder.ToTable("texts", "streetcode");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.TextContent)
                .IsRequired()
                .HasMaxLength(15000);

            builder.Property(t => t.Author)
                .HasMaxLength(200);

            builder.Property(t => t.VideoUrl)
                .HasMaxLength(500);

            builder.Property(t => t.AdditionalText).HasMaxLength(500);
        }
    }
}
