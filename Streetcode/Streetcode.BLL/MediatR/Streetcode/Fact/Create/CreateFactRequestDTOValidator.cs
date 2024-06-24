using FluentValidation;
namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create;

public class CreateFactRequestDTOValidator : AbstractValidator<CreateFactCommand>
{
    public CreateFactRequestDTOValidator()
    { 
        RuleFor(x => x.NewFact).NotEmpty();
        RuleFor(x => x.NewFact.Title).NotEmpty();
        RuleFor(x => x.NewFact.FactContent).NotEmpty();
        RuleFor(x => x.NewFact.StreetcodeId).GreaterThan(0);
        RuleFor(x => x.NewFact.ImageId).GreaterThan(0);
    }
}