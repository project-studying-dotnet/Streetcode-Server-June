using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates;
namespace Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;

public class CreateCoordinateRequestDTOValidator : AbstractValidator<CreateCoordinateCommand>
{
    public CreateCoordinateRequestDTOValidator()
    {
        RuleFor(x => x.StreetcodeCoordinate.Latitude).GreaterThan(-90).LessThan(90);
        RuleFor(x => x.StreetcodeCoordinate.Longtitude).GreaterThan(-180).LessThan(180);
    }
}