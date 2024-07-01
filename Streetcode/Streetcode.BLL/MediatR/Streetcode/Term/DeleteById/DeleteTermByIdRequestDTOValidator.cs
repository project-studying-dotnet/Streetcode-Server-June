using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Term.DeleteById;

public class DeleteTermByIdRequestDTOValidator : AbstractValidator<DeleteTermByIdCommand>
{
    public DeleteTermByIdRequestDTOValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}