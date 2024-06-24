using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedFigure.Delete;

public class DeleteRelatedFigureRequestDTOValidator : AbstractValidator<DeleteRelatedFigureCommand>
{
    public DeleteRelatedFigureRequestDTOValidator()
    {
        RuleFor(x => x.ObserverId).GreaterThan(0);
        RuleFor(x => x.TargetId).GreaterThan(0);
    }
}