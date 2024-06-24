using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Create;
public record CreateStreetcodeCommand(CreateStreetcodeDTO newStreetcode)
    : IValidatableRequest<Result<CreateStreetcodeDTO>>;
