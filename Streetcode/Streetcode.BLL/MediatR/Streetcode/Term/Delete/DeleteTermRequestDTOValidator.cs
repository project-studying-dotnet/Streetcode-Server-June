using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Delete;

public class DeleteTermRequestDTOValidator : AbstractValidator<DeleteTermCommand>
{
    public DeleteTermRequestDTOValidator()
    {
        RuleFor(x => x.Title).NotEmpty().Length(1, 50);
    }
}