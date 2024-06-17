using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Team;

namespace Streetcode.DAL.Persistence.Configurations.Team
{
    internal class TeamMemberPositionsConfiguration : IEntityTypeConfiguration<TeamMemberPositions>
    {
        public void Configure(EntityTypeBuilder<TeamMemberPositions> builder)
        {
            builder.ToTable("team_member_positions", "team");

            builder
                .HasKey(nameof(TeamMemberPositions.TeamMemberId), nameof(TeamMemberPositions.PositionsId));
        }
    }
}
