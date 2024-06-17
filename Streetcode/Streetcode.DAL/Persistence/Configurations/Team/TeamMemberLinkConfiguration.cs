using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Team;

namespace Streetcode.DAL.Persistence.Configurations.Team
{
    internal class TeamMemberLinkConfiguration : IEntityTypeConfiguration<TeamMemberLink>
    {
        public void Configure(EntityTypeBuilder<TeamMemberLink> builder)
        {
            builder.ToTable("team_member_links", "team");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.LogoType).IsRequired();

            builder.Property(t => t.TargetUrl)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}
