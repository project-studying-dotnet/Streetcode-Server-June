using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.DTO.Sources;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.UpdateContent
{
    public record CategoryContentUpdateCommand(CategoryContentUpdateDTO updatedContent) : IRequest<Result<StreetcodeCategoryContentDTO>>;
}
