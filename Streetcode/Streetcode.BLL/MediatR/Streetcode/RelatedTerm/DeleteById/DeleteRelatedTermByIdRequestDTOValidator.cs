using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.DeleteById;

public class DeleteRelatedTermByIdRequestDTOValidator : AbstractValidator<DeleteRelatedTermByIdCommand>
{
    public DeleteRelatedTermByIdRequestDTOValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}