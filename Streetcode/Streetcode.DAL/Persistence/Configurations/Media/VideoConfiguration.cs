using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Media;

namespace Streetcode.DAL.Persistence.Configurations.Media
{
    internal class VideoConfiguration : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.ToTable("videos", "media");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id).ValueGeneratedOnAdd();

            builder.Property(v => v.Title).HasMaxLength(100);

            builder.Property(v => v.Url).IsRequired();
        }
    }
}
