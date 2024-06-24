using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Facts.Update;

public class UpdateFactRequestDTOValidator : AbstractValidator<UpdateFactCommand>
{
    public UpdateFactRequestDTOValidator()
    {
        RuleFor(x => x.Fact).NotEmpty();
        RuleFor(x => x.Fact.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Fact.FactContent).NotEmpty().MaximumLength(1500);
        RuleFor(x => x.Fact.StreetcodeId).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Fact.ImageId).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Fact.ImageDescription).NotEmpty().MaximumLength(200);
    }
}