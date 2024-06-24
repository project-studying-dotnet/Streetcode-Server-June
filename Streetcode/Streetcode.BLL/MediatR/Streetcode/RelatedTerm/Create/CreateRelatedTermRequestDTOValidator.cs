using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;

public class CreateRelatedTermRequestDTOValidator : AbstractValidator<CreateRelatedTermCommand>
{
    public CreateRelatedTermRequestDTOValidator()
    {
        RuleFor(x => x.RelatedTerm.TermId).GreaterThan(0);
        RuleFor(x => x.RelatedTerm.Word).NotEmpty().Length(1, 50);
    }
}