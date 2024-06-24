using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.AdditionalContent.Tag.Create
{
  public record CreateTagCommand(CreateTagDTO tag) : IValidatableRequest<Result<TagDTO>>;
}
