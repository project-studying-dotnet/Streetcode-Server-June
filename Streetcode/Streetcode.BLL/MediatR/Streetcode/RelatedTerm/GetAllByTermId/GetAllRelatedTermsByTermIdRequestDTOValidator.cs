using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;

public class GetAllRelatedTermsByTermIdRequestDTOValidator : AbstractValidator<GetAllRelatedTermsByTermIdQuery>
{
    public GetAllRelatedTermsByTermIdRequestDTOValidator()
    {
        RuleFor(x => x.TermId).GreaterThan(0);
    }
}