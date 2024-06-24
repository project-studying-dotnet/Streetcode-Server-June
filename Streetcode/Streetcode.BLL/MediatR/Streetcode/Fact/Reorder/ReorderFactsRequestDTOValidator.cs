using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;

public class ReorderFactsRequestDTOValidator : AbstractValidator<ReorderFactsCommand>
{
    public ReorderFactsRequestDTOValidator()
    {
        RuleFor(x => x.newPositionsOfFacts).NotEmpty();
        RuleFor(x => x.streetcodeId).NotEmpty().GreaterThan(0);
    }
}