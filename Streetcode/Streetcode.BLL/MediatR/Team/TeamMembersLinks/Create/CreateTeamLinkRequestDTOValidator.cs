using FluentValidation;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;

public class CreateTeamLinkRequestDTOValidator : AbstractValidator<CreateTeamLinkCommand>
{
    public CreateTeamLinkRequestDTOValidator()
    {
        RuleFor(x => x.teamMember).NotEmpty();
        RuleFor(x => x.teamMember.TeamMemberId).GreaterThan(0);
        RuleFor(x => x.teamMember.LogoType).IsInEnum();
    }
}