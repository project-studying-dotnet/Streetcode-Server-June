using FluentValidation;

namespace Streetcode.BLL.MediatR.Media.Art.Create
{
    public class CreateArtRequestDTOValidator : AbstractValidator<CreateArtCommand>
    {
        public CreateArtRequestDTOValidator()
        {
            RuleFor(x => x.Art.Title).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Art.Description).NotEmpty().MaximumLength(400);
            RuleFor(x => x.Art.ImageId).NotEmpty().GreaterThan(0);
        }
    }
}