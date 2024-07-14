using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Likes.PushLike
{
    public class GetLikesByUserIdValidator : AbstractValidator<GetLikesByUserIdQuery>
    {
        public GetLikesByUserIdValidator()
        {
            RuleFor(x => x.userId).NotEmpty();
        }
    }
}
