using FluentValidation;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.UpdateContent;

public class CategoryContentUpdateRequestDTOValidator : AbstractValidator<CategoryContentUpdateCommand>
{
    public CategoryContentUpdateRequestDTOValidator()
    {
        RuleFor(x => x.updatedContent).NotEmpty();
        RuleFor(x => x.updatedContent.Text).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.updatedContent.SourceLinkCategoryId).NotEmpty().GreaterThan(0);
    }
}