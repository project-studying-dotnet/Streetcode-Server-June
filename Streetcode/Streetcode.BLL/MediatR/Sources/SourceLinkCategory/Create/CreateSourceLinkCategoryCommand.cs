using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.DTO.Sources.Create;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;

public record CreateSourceLinkCategoryCommand(CreateSourceCategoryDTO newCategory) : IRequest<Result<SourceLinkCategoryDTO>>;