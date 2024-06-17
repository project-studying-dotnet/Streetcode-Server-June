using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Partners;

namespace Streetcode.DAL.Persistence.Configurations.Partners
{
    internal class PartnerSourceLinkConfiguration : IEntityTypeConfiguration<PartnerSourceLink>
    {
        public void Configure(EntityTypeBuilder<PartnerSourceLink> builder)
        {
            builder.ToTable("partner_source_links", "partners");

            builder.HasKey(psl => psl.Id);

            builder.Property(psl => psl.Id).ValueGeneratedOnAdd();

            builder.Property(psl => psl.LogoType)
                .IsRequired();

            builder.Property(psl => psl.TargetUrl)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}
