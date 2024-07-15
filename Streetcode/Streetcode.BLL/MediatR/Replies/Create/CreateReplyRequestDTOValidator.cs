using FluentValidation;
using Streetcode.BLL.MediatR.Comments.Create;

namespace Streetcode.BLL.MediatR.Replies;

public class CreateReplyRequestDTOValidator
{
    public class CreateCommentRequestDTOValidator : AbstractValidator<CreateReplyCommand>
    {
        public CreateCommentRequestDTOValidator()
        {
            RuleFor(x => x.reply.StreetcodeId).NotEmpty().GreaterThan(0);
            RuleFor(x => x.reply.CommentContent).NotEmpty().MaximumLength(300);
            RuleFor(x => x.reply.ParentId).NotEmpty().GreaterThan(0);
        }
    }
}