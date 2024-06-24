using FluentValidation;

namespace Streetcode.BLL.MediatR.Newss.Update;

public class UpdateNewsRequestDTOValidator : AbstractValidator<UpdateNewsCommand>
{
    public UpdateNewsRequestDTOValidator()
    {
        RuleFor(x => x.news.ImageId).GreaterThan(0);
        RuleFor(x => x.news.Title).NotEmpty();
    }
}