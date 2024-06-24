using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Text.DeleteById;

public class DeleteTextByIdRequestDTOValidator : AbstractValidator<DeleteTextByIdCommand>
{
    public DeleteTextByIdRequestDTOValidator()
    {
        RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
    }
}