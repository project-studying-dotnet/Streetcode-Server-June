using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;

public class DeleteRelatedTermRequestDTOValidator : AbstractValidator<DeleteRelatedTermCommand>
{
    public DeleteRelatedTermRequestDTOValidator()
    {
        RuleFor(x => x.word).NotEmpty().Length(1, 50);
    }
}