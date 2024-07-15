using FluentValidation;

namespace Streetcode.BLL.MediatR.Comments.Delete
{
    public class DeleteCommentRequestDTOValidator : AbstractValidator<DeleteCommentCommand>
    {
        public DeleteCommentRequestDTOValidator()
        {
            RuleFor(x => x.commentId).GreaterThan(0);
        }
    }
}
