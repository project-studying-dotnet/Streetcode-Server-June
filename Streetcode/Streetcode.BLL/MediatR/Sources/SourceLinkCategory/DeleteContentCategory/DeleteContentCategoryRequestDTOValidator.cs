using FluentValidation;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteContentCategory;

public class DeleteContentCategoryRequestDTOValidator : AbstractValidator<DeleteContentCategoryCommand>
{
    public DeleteContentCategoryRequestDTOValidator()
    {
        RuleFor(x => x.sourcelinkcatId).NotEmpty().GreaterThan(0);
        RuleFor(x => x.streetcodeId).NotEmpty().GreaterThan(0);
    }
}