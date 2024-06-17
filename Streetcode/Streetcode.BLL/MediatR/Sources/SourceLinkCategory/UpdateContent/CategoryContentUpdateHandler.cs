using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.UpdateContent;

public class CategoryContentUpdateHandler : IRequestHandler<CategoryContentUpdateCommand, Result<StreetcodeCategoryContentDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CategoryContentUpdateHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<StreetcodeCategoryContentDTO>> Handle(CategoryContentUpdateCommand request, CancellationToken cancellationToken)
    {
        var updatedContent = _mapper.Map<StreetcodeCategoryContent>(request.updatedContent);
        try
        {
            if (updatedContent is null)
            {
                const string errorMsg = "content is null";
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            var content = _repositoryWrapper.StreetcodeCategoryContentRepository.Update(updatedContent);

             // _repositoryWrapper.StreetcodeCategoryContentRepository
                            //                .GetFirstOrDefaultAsync(u => u.StreetcodeId == updatedContent.StreetcodeId &&
                                                                         // u.SourceLinkCategoryId == updatedContent.SourceLinkCategoryId);
            // if(content.Result.Texts != updatedContent.Texts)
            // {
            //    string errorMsg = "Failed to update content";
            //    _logger.LogError(request, errorMsg);
            //    return Result.Fail(errorMsg);
            // }

            _repositoryWrapper.SaveChanges();

            return Result.Ok(_mapper.Map<StreetcodeCategoryContentDTO>(updatedContent));
        }
        catch (Exception ex)
        {
            string errorMsg = "Failed to update content";
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }
    }
}