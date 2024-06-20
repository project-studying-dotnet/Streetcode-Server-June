using FluentValidation;

namespace Streetcode.BLL.MediatR.Team.Create;

public class CreatePositionRequestDTOValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionRequestDTOValidator()
    {
        RuleFor(x => x.position).NotNull();
        RuleFor(x => x.position.Position).MaximumLength(50);
    }
}