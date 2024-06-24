using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update
{
    public record UpdateTimelineItemCommand(TimelineItemDTO sourceTimeLine) : IValidatableRequest<Result<TimelineItemDTO>>;
}
