using FluentValidation;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;

public class CreateSourceLinkCategoryRequestDTOValidator : AbstractValidator<CreateSourceLinkCategoryCommand>
{
    public CreateSourceLinkCategoryRequestDTOValidator()
    {
        RuleFor(x => x.newCategory).NotEmpty();
        RuleFor(x => x.newCategory.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.newCategory.ImageId).NotEmpty().GreaterThan(0);
    }
}