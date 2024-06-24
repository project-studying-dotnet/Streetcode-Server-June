using FluentValidation;

namespace Streetcode.BLL.MediatR.Toponyms.StreetCodeRecord.Delete;

public class DeleteStreetcodeRecordRequestDTOValidator : AbstractValidator<DeleteStreetcodeRecordCommand>
{
    public DeleteStreetcodeRecordRequestDTOValidator()
    {
        RuleFor(x => x.StreetcodeId).NotEmpty().GreaterThan(0);
        RuleFor(x => x.ToponymId).NotEmpty().GreaterThan(0);
    }
}