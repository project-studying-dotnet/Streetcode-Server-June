using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Persistence.Configurations.Media.Images
{
    internal class ImageDetailsConfiguration : IEntityTypeConfiguration<ImageDetails>
    {
        public void Configure(EntityTypeBuilder<ImageDetails> builder)
        {
            builder.ToTable("image_details", "media");
            
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id).ValueGeneratedOnAdd();

            builder.Property(i => i.Title).HasMaxLength(100);

            builder.Property(i => i.Alt).HasMaxLength(300);
        }
    }
}
