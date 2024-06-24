using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Art;

namespace Streetcode.BLL.MediatR.Media.Art.Delete
{
    public record DeleteArtCommand(int id) : IRequest<Result<ArtDTO>>;
}