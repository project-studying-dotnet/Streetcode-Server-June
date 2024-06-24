using FluentValidation;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update
{
    public class UpdateTimelineItemRequestDTOValidator : AbstractValidator<UpdateTimelineItemCommand>
    {
        public UpdateTimelineItemRequestDTOValidator()
        {
            RuleFor(x => x.sourceTimeLine).NotEmpty();
            RuleFor(x => x.sourceTimeLine.Id).NotEmpty().GreaterThan(0);
            RuleFor(x => x.sourceTimeLine.Title).MaximumLength(26);
            RuleFor(x => x.sourceTimeLine.Description).MaximumLength(400);
            RuleFor(x => x.sourceTimeLine.Date).NotEmpty();
            RuleFor(x => x.sourceTimeLine.DateViewPattern).NotEmpty();
        }
    }
}
