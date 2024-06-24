using FluentValidation;

namespace Streetcode.BLL.MediatR.Partners.Update;

public class UpdatePartnerRequestDTOValidator : AbstractValidator<UpdatePartnerCommand>
{
    public UpdatePartnerRequestDTOValidator()
    {
        RuleFor(x => x.Partner.Title).NotEmpty();
        RuleFor(x => x.Partner.Description).NotEmpty();
        RuleFor(x => x.Partner.LogoId).GreaterThan(0);
    }
}