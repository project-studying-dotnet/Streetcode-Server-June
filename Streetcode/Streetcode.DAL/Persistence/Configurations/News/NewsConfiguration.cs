using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.News;

namespace Streetcode.DAL.Persistence.Configurations.NewsConf
{
    internal class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.ToTable("news", "news");

            builder.HasIndex(n => n.URL).IsUnique();

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(n => n.Text).IsRequired();

            builder.Property(n => n.URL)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(n => n.CreationDate).IsRequired();

            builder.HasOne(x => x.Image)
                .WithOne(x => x.News)
                .HasForeignKey<News>(x => x.ImageId);
        }
    }
}
