using FluentValidation;

namespace Streetcode.BLL.MediatR.Replies.Delete
{
    public class DeleteReplyRequestDTOValidator : AbstractValidator<DeleteReplyCommand>
    {
        public DeleteReplyRequestDTOValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
