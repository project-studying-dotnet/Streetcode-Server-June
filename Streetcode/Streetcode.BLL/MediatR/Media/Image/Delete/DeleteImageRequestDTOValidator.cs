using FluentValidation;

namespace Streetcode.BLL.MediatR.Media.Image.Delete;

public class DeleteImageRequestDTOValidator : AbstractValidator<DeleteImageCommand>
{
        public DeleteImageRequestDTOValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
}