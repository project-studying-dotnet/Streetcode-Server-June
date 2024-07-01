using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Term.GetById;

public class GetTermByIdRequestDTOValidator : AbstractValidator<GetTermByIdQuery>
{
    public GetTermByIdRequestDTOValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}