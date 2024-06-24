using FluentValidation;

namespace Streetcode.BLL.MediatR.Payment;

public class CreateInvoiceRequestDTOValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceRequestDTOValidator()
    {
        RuleFor(x => x.Payment.Amount).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Payment.RedirectUrl).NotEmpty();
    }
}