using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;

public class DeleteRelatedTermRequestDTOValidator : AbstractValidator<DeleteRelatedTermCommand>
{
    public DeleteRelatedTermRequestDTOValidator()
    {
        RuleFor(x => x.Word).NotEmpty().Length(1, 50);
    }
}