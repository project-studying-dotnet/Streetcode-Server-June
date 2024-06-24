using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Media.Art;

namespace Streetcode.BLL.MediatR.Media.Art.Create
{
    public record CreateArtCommand(ArtCreateUpdateDTO Art) : IValidatableRequest<Result<ArtCreateUpdateDTO>>;
}
