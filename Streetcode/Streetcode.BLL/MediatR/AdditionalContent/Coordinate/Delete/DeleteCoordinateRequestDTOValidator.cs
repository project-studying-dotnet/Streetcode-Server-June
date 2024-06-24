using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates;
namespace Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Delete;

public class DeleteCoordinateRequestDTOValidator : AbstractValidator<DeleteCoordinateCommand>
{
    public DeleteCoordinateRequestDTOValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}