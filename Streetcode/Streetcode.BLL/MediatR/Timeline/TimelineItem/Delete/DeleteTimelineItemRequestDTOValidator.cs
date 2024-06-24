using FluentValidation;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;

public class DeleteTimelineItemRequestDTOValidator : AbstractValidator<DeleteTimelineItemCommand>
{
    public DeleteTimelineItemRequestDTOValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}