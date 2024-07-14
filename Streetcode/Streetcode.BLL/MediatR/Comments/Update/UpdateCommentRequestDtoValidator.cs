using FluentValidation;
using Streetcode.BLL.Util.Comments;

namespace Streetcode.BLL.MediatR.Comments.Update;

public class UpdateCommentRequestDtoValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentRequestDtoValidator()
    {
        RuleFor(cm => cm.EditedComment.Id).NotNull().GreaterThan(0);
        RuleFor(cm => cm.EditedComment.CommentContent).NotNull().NotEmpty().Must(BadWordsVerification.NotContainBadWords);
    }
}
