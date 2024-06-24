using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedFigure.Create;

public class CreateRelatedFigureRequestDTOValidator : AbstractValidator<CreateRelatedFigureCommand>
{
    public CreateRelatedFigureRequestDTOValidator()
    {
        RuleFor(x => x.ObserverId).GreaterThan(0);
        RuleFor(x => x.TargetId).GreaterThan(0);
    }
}