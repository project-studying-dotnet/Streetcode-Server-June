using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.Create;

namespace Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Create;

public class CreateTagRequestDTOValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagRequestDTOValidator()
    {
        RuleFor(dto => dto.tag.Title).NotEmpty().MaximumLength(50);
    }
}