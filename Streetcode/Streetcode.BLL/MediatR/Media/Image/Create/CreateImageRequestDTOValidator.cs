using FluentValidation;

namespace Streetcode.BLL.MediatR.Media.Image.Create;

public class CreateImageRequestDTOValidator : AbstractValidator<CreateImageCommand>
{
    public CreateImageRequestDTOValidator()
    {
        RuleFor(x => x.Image.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Image.MimeType).NotEmpty();
        RuleFor(x => x.Image.Alt).NotEmpty().MaximumLength(200);
    }
}