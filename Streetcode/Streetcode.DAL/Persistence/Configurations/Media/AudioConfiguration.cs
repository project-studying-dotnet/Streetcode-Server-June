using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Media;

namespace Streetcode.DAL.Persistence.Configurations.Media
{
    internal class AudioConfigurationinternal : IEntityTypeConfiguration<Audio>
    {
        public void Configure(EntityTypeBuilder<Audio> builder)
        {
            builder.ToTable("audios", "media");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(a => a.Title).HasMaxLength(100);

            builder.Property(a => a.BlobName).IsRequired().HasMaxLength(100);

            builder.Property(a => a.MimeType).IsRequired().HasMaxLength(10);

            builder.Ignore(a => a.Base64);
        }
    }
}
