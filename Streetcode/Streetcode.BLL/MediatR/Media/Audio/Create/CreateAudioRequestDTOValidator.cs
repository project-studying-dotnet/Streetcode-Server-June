using FluentValidation;
namespace Streetcode.BLL.MediatR.Media.Audio.Create;

public class CreateAudioRequestDTOValidator : AbstractValidator<CreateAudioCommand>
{
    public CreateAudioRequestDTOValidator()
    {
        RuleFor(x => x.Audio.Description).MaximumLength(200);
        RuleFor(x => x.Audio.Title).MaximumLength(100);
        RuleFor(x => x.Audio.Extension).NotEmpty();
        RuleFor(x => x.Audio.BaseFormat).NotEmpty();
        RuleFor(x => x.Audio.MimeType).NotEmpty();
    }
}