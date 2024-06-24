using FluentValidation;

namespace Streetcode.BLL.MediatR.Newss.Delete;

public class DeleteNewsRequestDTOValidator : AbstractValidator<DeleteNewsCommand>
{
    public DeleteNewsRequestDTOValidator()
    {
        RuleFor(x => x.id).GreaterThan(0);
    }
}