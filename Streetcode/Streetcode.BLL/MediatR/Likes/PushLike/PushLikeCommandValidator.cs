using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Likes.PushLike
{
    public class PushLikeCommandValidator : AbstractValidator<PushLikeCommand>
    {
        public PushLikeCommandValidator()
        {
            RuleFor(x => x.pushLike.UserId).NotEmpty();
            RuleFor(x => x.pushLike.streetcodeId).GreaterThan(0);
        }
    }
}
