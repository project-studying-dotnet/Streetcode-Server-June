using FluentValidation;
namespace Streetcode.BLL.MediatR.Media.Audio.Delete;

public class DeleteAudioRequestDTOValidator : AbstractValidator<DeleteAudioCommand>
{
    public DeleteAudioRequestDTOValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}