using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Media.Art;

namespace Streetcode.BLL.MediatR.Media.Art.Update
{
    public record UpdateArtCommand(ArtCreateUpdateDTO ArtDto) : IValidatableRequest<Result<ArtCreateUpdateDTO>>;
}
