using FluentValidation;

namespace Streetcode.BLL.MediatR.Partners.Create;

public class CreatePartnerRequestDTOValidator : AbstractValidator<CreatePartnerCommand>
{
    public CreatePartnerRequestDTOValidator()
    {
        RuleFor(x => x.newPartner.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.newPartner.TargetUrl).MaximumLength(200);
        RuleFor(x => x.newPartner.UrlTitle).MaximumLength(100);
        RuleFor(x => x.newPartner.Description).MaximumLength(450);
    }
}