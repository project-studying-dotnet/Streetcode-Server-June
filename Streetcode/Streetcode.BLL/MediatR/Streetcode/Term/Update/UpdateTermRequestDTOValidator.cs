using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Update;

public class UpdateTermRequestDTOValidator : AbstractValidator<UpdateTermCommand>
{
    public UpdateTermRequestDTOValidator()
    {
        RuleFor(x => x.Term.Id).GreaterThan(0);
        RuleFor(x => x.Term.Title).Length(1, 50);
        RuleFor(x => x.Term.Description).NotEmpty().Length(1, 500);
    }
}