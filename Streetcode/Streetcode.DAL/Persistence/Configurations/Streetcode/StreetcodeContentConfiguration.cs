using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Entities.Streetcode.Types;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Entities.Transactions;
using Streetcode.DAL.Enums;

namespace Streetcode.DAL.Persistence.Configurations.Streetcode
{
    internal class StreetcodeContentConfiguration : IEntityTypeConfiguration<StreetcodeContent>
    {
        public void Configure(EntityTypeBuilder<StreetcodeContent> builder)
        {
            builder.ToTable("streetcodes", "streetcode");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            builder.Property(s => s.Index).IsRequired();

            builder.Property(s => s.Teaser).HasMaxLength(650);

            builder.Property(s => s.DateString)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Alias).HasMaxLength(50);

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.TransliterationUrl)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(e => e.TransliterationUrl)
                .IsUnique();

            builder.HasIndex(e => e.Index)
                .IsUnique();

            builder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(s => s.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(s => s.ViewCount)
                .HasDefaultValue(0);

            builder.Property(s => s.EventStartOrPersonBirthDate).IsRequired();

            builder.HasDiscriminator<string>(StreetcodeTypeDiscriminators.DiscriminatorName)
                .HasValue<StreetcodeContent>(StreetcodeTypeDiscriminators.StreetcodeBaseType)
                .HasValue<PersonStreetcode>(StreetcodeTypeDiscriminators.StreetcodePersonType)
                .HasValue<EventStreetcode>(StreetcodeTypeDiscriminators.StreetcodeEventType);

            builder.Property<string>("StreetcodeType").Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);

            builder.HasOne(d => d.Text)
                    .WithOne(p => p.Streetcode)
                    .HasForeignKey<Text>(d => d.StreetcodeId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Audio)
                    .WithOne(p => p.Streetcode)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.StatisticRecords)
                    .WithOne(t => t.Streetcode)
                    .HasForeignKey(t => t.StreetcodeId)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.TransactionLink)
                    .WithOne(p => p.Streetcode)
                    .HasForeignKey<TransactionLink>(d => d.StreetcodeId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Images)
                    .WithMany(i => i.Streetcodes)
                    .UsingEntity<StreetcodeImage>(
                    si => si.HasOne(i => i.Image).WithMany().HasForeignKey(i => i.ImageId),
                    si => si.HasOne(i => i.Streetcode).WithMany().HasForeignKey(i => i.StreetcodeId))
                    .ToTable("streetcode_image", "streetcode");

            builder.HasMany(s => s.Subtitles)
                .WithOne(s => s.Streetcode)
                .HasForeignKey(s => s.StreetcodeId);

            builder.HasMany(d => d.Facts)
                .WithOne(f => f.Streetcode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(f => f.StreetcodeId);

            builder.HasMany(d => d.Videos)
                    .WithOne(p => p.Streetcode)
                    .HasForeignKey(d => d.StreetcodeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(d => d.StreetcodeId);

            builder.HasMany(d => d.SourceLinkCategories)
                    .WithMany(c => c.Streetcodes)
                    .UsingEntity<StreetcodeCategoryContent>(
                        scat => scat.HasOne(i => i.SourceLinkCategory).WithMany(s => s.StreetcodeCategoryContents).HasForeignKey(i => i.SourceLinkCategoryId),
                        scat => scat.HasOne(i => i.Streetcode).WithMany(s => s.StreetcodeCategoryContents).HasForeignKey(i => i.StreetcodeId))
                    .ToTable("streetcode_source_link_categories", "sources");

            builder.HasMany(d => d.TimelineItems)
                .WithOne(t => t.Streetcode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(t => t.StreetcodeId);

            builder.HasMany(d => d.Partners)
                    .WithMany(p => p.Streetcodes)
                    .UsingEntity<StreetcodePartner>(
                        sp => sp.HasOne(i => i.Partner).WithMany().HasForeignKey(x => x.PartnerId),
                        sp => sp.HasOne(i => i.Streetcode).WithMany().HasForeignKey(x => x.StreetcodeId))
                   .ToTable("streetcode_partners", "streetcode");

            builder.HasMany(d => d.Coordinates)
                .WithOne(c => c.Streetcode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(c => c.StreetcodeId);

            builder.HasMany(d => d.Toponyms)
                .WithMany(t => t.Streetcodes)
                .UsingEntity<StreetcodeToponym>(
                    st => st.HasOne(s => s.Toponym).WithMany().HasForeignKey(x => x.ToponymId),
                    st => st.HasOne(s => s.Streetcode).WithMany().HasForeignKey(x => x.StreetcodeId))
                .ToTable("streetcode_toponym", "streetcode");
        }
    }
}
