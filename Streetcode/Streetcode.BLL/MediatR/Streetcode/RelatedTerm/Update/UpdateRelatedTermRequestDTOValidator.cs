using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;

public class UpdateRelatedTermRequestDTOValidator : AbstractValidator<UpdateRelatedTermCommand>
{
    public UpdateRelatedTermRequestDTOValidator()
    {
        RuleFor(x => x.RelatedTerm.Id).GreaterThan(0);
        RuleFor(x => x.RelatedTerm.TermId).GreaterThan(0);
        RuleFor(x => x.RelatedTerm.Word).NotEmpty().Length(1, 50);
    }
}