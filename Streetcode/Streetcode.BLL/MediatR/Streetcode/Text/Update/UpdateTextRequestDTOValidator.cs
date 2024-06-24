using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Update;

public class UpdateTextRequestDTOValidator : AbstractValidator<UpdateTextCommand>
{
    public UpdateTextRequestDTOValidator()
    {
        RuleFor(x => x.TextUpdate).NotEmpty();
        RuleFor(x => x.TextUpdate.Title).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TextUpdate.TextContent).NotEmpty().MaximumLength(15000);
        RuleFor(x => x.TextUpdate.AdditionalText).MaximumLength(500);
        RuleFor(x => x.TextUpdate.VideoUrl).MaximumLength(500);
        RuleFor(x => x.TextUpdate.Author).MaximumLength(200);
        RuleFor(x => x.TextUpdate.StreetcodeId).NotEmpty().GreaterThan(0);
    }
}