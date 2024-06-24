using FluentValidation;

namespace Streetcode.BLL.MediatR.Partners.Delete;

public class DeletePartnerRequestDTOValidator : AbstractValidator<DeletePartnerCommand>
{
    public DeletePartnerRequestDTOValidator()
    {
        RuleFor(x => x.id).GreaterThan(0);
    }
}