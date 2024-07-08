using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.MediatR.Toponyms.StreetCodeRecord.Create;
using Streetcode.BLL.MediatR.Toponyms.StreetCodeRecord.Delete;

namespace Streetcode.WebApi.Controllers.Toponyms
{
    public class StreetCodeRecordController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StreetcodeRecordDTO fact)
        {
            return HandleResult(await Mediator.Send(new CreateStreetcodeRecordCommand(fact)));
        }

        [HttpDelete("{StreetcodeId:int}/{ToponymId:int}", Name = "DeleteRecord")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteRecord(int StreetcodeId, int ToponymId)
        {
            return HandleResult(await Mediator.Send(new DeleteStreetcodeRecordCommand(StreetcodeId, ToponymId)));
        }
    }
}
