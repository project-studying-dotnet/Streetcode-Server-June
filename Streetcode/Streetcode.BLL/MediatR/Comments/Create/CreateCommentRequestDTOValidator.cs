using FluentValidation;
using Streetcode.BLL.Util.Comments;

namespace Streetcode.BLL.MediatR.Comments.Create
{
    public class CreateCommentRequestDTOValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentRequestDTOValidator()
        {
            RuleFor(x => x.comment.StreetcodeId).NotEmpty().GreaterThan(0);
            RuleFor(x => x.comment.CommentContent).NotEmpty().MaximumLength(300);
            RuleFor(x => x.comment.CommentContent).NotEmpty().Must(BadWordsVerification.NotContainBadWords);
        }
    }
}
