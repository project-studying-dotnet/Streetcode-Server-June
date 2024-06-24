using FluentValidation;

namespace Streetcode.BLL.MediatR.Media.Art.Update
{
    public class UpdateArtRequestDTOValidator : AbstractValidator<UpdateArtCommand>
    {
        public UpdateArtRequestDTOValidator()
        {
            RuleFor(x => x.ArtDto.Id).NotEmpty().GreaterThan(0);
            RuleFor(x => x.ArtDto.Title).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ArtDto.Description).NotEmpty().MaximumLength(400);
            RuleFor(x => x.ArtDto.ImageId).NotEmpty().GreaterThan(0);
        }
    }
}
