using FluentValidation;
using Streetcode.BLL.Util;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Create;

public class CreateTextRequestDTOValidator : AbstractValidator<CreateTextCommand>
{
    public CreateTextRequestDTOValidator()
    {
        RuleFor(x => x.TextCreate).NotEmpty();
        RuleFor(x => x.TextCreate.Title).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TextCreate.TextContent).NotEmpty().MaximumLength(15000);
        RuleFor(x => x.TextCreate.AdditionalText).MaximumLength(500);
        RuleFor(x => x.TextCreate.VideoUrl).MaximumLength(500);
        RuleFor(x => x.TextCreate.Author).MaximumLength(200);
        RuleFor(x => x.TextCreate.StreetcodeId).NotEmpty().GreaterThan(0);
    }
}
