using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.Toponyms.StreetCodeRecord.Delete
{
    public record DeleteStreetcodeRecordCommand(int StreetcodeId, int ToponymId) : IValidatableRequest<Result<StreetcodeRecordDTO>>;
}
