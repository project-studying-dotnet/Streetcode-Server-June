using FluentResults;
using MediatR;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;

public record DeleteTimelineItemCommand(int Id) : IRequest<Result<Unit>>;
