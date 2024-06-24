using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteContentCategory
{
    public record DeleteContentCategoryCommand(int sourcelinkcatId, int streetcodeId) : IValidatableRequest<Result<StreetcodeCategoryContentDTO>>;
}
