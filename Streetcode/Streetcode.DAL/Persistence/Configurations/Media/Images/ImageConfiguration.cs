using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Partners;

namespace Streetcode.DAL.Persistence.Configurations.Media.Images
{
    internal class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("images", "media");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id).ValueGeneratedOnAdd();

            builder.Ignore(i => i.Base64);

            builder.Property(i => i.BlobName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.MimeType)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasOne(d => d.Art)
                .WithOne(a => a.Image)
                .HasForeignKey<Art>(a => a.ImageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(im => im.ImageDetails)
                .WithOne(info => info.Image)
                .HasForeignKey<ImageDetails>(a => a.ImageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Partner)
                .WithOne(p => p.Logo)
                .HasForeignKey<Partner>(d => d.LogoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Facts)
                .WithOne(p => p.Image)
                .HasForeignKey(d => d.ImageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.SourceLinkCategories)
                .WithOne(s => s.Image)
                .HasForeignKey(d => d.ImageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
