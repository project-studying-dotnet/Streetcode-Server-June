using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create
{
    public record CreateTeamLinkCommand(TeamMemberLinkDTO teamMember) : IValidatableRequest<Result<TeamMemberLinkDTO>>;
}
