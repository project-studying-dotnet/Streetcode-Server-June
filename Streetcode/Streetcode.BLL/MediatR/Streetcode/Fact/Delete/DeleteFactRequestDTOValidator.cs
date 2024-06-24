using FluentValidation;
using Streetcode.BLL.MediatR.Partners.Delete;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Delete;

public class DeleteFactRequestDTOValidator : AbstractValidator<DeleteFactCommand>
{
    public DeleteFactRequestDTOValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}