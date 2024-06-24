using FluentValidation;

namespace Streetcode.BLL.MediatR.Toponyms.StreetCodeRecord.Create;

public class CreateStreetcodeRecordRequestDTOValidator : AbstractValidator<CreateStreetcodeRecordCommand>
{
    public CreateStreetcodeRecordRequestDTOValidator()
    {
        RuleFor(x => x.newRecord).NotEmpty();
        RuleFor(x => x.newRecord.StreetcodeId).NotEmpty().GreaterThan(0);
        RuleFor(x => x.newRecord.ToponymId).NotEmpty().GreaterThan(0);
    }
}