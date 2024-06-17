using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Partners;

namespace Streetcode.DAL.Persistence.Configurations.Partners
{
    internal class PartnerConfiguration : IEntityTypeConfiguration<Partner>
    {
        public void Configure(EntityTypeBuilder<Partner> builder)
        {
            builder.ToTable("partners", "partners");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.IsKeyPartner)
                .IsRequired()
                .HasDefaultValue("false");

            builder.Property(p => p.IsVisibleEverywhere).IsRequired();

            builder.Property(p => p.TargetUrl).HasMaxLength(255);

            builder.Property(p => p.UrlTitle).HasMaxLength(255);

            builder.Property(p => p.Description).HasMaxLength(600);

            builder.HasMany(d => d.PartnerSourceLinks)
                .WithOne(p => p.Partner)
                .HasForeignKey(d => d.PartnerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
