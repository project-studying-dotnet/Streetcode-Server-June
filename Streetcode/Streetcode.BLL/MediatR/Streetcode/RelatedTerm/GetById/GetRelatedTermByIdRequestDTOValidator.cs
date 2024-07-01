using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById;

public class GetRelatedTermByIdRequestDTOValidator : AbstractValidator<GetRelatedTermByIdQuery>
{
    public GetRelatedTermByIdRequestDTOValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}