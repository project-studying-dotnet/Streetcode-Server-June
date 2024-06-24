using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.DeleteSoft;

public class DeleteSoftStreetcodeRequestDTOValidator : AbstractValidator<DeleteSoftStreetcodeCommand>
{
    public DeleteSoftStreetcodeRequestDTOValidator()
    {
        RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
    }
}