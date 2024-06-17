using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Team;

namespace Streetcode.DAL.Persistence.Configurations.Team
{
    internal class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
    {
        public void Configure(EntityTypeBuilder<TeamMember> builder)
        {
            builder.ToTable("team_members", "team");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.LastName)
                .HasMaxLength(50);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(P => P.IsMain).IsRequired();

            builder
                .HasOne(x => x.Image)
                .WithOne(x => x.TeamMember)
                .HasForeignKey<TeamMember>(x => x.ImageId);

            builder
                .HasMany(x => x.Positions)
                .WithMany(x => x.TeamMembers)
                .UsingEntity<TeamMemberPositions>(
                tp => tp.HasOne(x => x.Positions).WithMany().HasForeignKey(x => x.PositionsId),
                tp => tp.HasOne(x => x.TeamMember).WithMany().HasForeignKey(x => x.TeamMemberId));

            builder
                .HasMany(x => x.TeamMemberLinks)
                .WithOne(x => x.TeamMember)
                .HasForeignKey(x => x.TeamMemberId);
        }
    }
}
