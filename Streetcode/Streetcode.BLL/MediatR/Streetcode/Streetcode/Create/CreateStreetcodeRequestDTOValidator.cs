using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Create
{
    public class CreateStreetcodeRequestDTOValidator : AbstractValidator<CreateStreetcodeCommand>
    {
        public CreateStreetcodeRequestDTOValidator()
        {
            RuleFor(x => x.newStreetcode.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.newStreetcode.FirstName).MaximumLength(50);
            RuleFor(x => x.newStreetcode.LastName).MaximumLength(50);
            RuleFor(x => x.newStreetcode.Rank).MaximumLength(50);
            RuleFor(x => x.newStreetcode.Alias).NotEmpty().MaximumLength(33);
            RuleFor(x => x.newStreetcode.TransliterationUrl).MaximumLength(100);
            RuleFor(x => x.newStreetcode.Teaser).MaximumLength(450);
            RuleFor(x => x.newStreetcode.TagIds).Must(m => m.Count() <= 50);
        }
    }
}
