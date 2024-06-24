using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates;
namespace Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Update;

public class UpdateCoordinateRequestDTOValidator : AbstractValidator<UpdateCoordinateCommand>
{
    public UpdateCoordinateRequestDTOValidator()
    {
        RuleFor(x => x.StreetcodeCoordinate.StreetcodeId).GreaterThan(0);
        RuleFor(x => x.StreetcodeCoordinate.Latitude).GreaterThan(-90).LessThan(90);
        RuleFor(x => x.StreetcodeCoordinate.Longtitude).GreaterThan(-180).LessThan(180);
    }
}