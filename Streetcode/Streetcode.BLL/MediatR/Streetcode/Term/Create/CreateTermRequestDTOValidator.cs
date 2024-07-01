using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Create;

public class CreateTermRequestDTOValidator : AbstractValidator<CreateTermCommand>
{
    public CreateTermRequestDTOValidator()
    {
        RuleFor(x => x.Term.Title).NotEmpty().Length(1, 50);
        RuleFor(x => x.Term.Description).NotEmpty().Length(1, 500);
    }
}