using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetAll;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;

namespace Streetcode.WebApi.Controllers.Timeline;

public class TimelineItemController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllTimelineItemsQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetTimelineItemByIdQuery(id)));
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetTimelineItemsByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Create([FromBody] CreateTimelineItemDTO timelineItem)
    {
        return HandleResult(await Mediator.Send(new CreateTimelineItemCommand(timelineItem)));
    }

    [HttpPut]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Update([FromBody] TimelineItemDTO timelineItem)
    {
        return HandleResult(await Mediator.Send(new UpdateTimelineItemCommand(timelineItem)));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteTimelineItemCommand(id)));
    }
}