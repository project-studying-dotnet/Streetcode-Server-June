using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;

public class CreateSourceLinkCategoryHandler : IRequestHandler<CreateSourceLinkCategoryCommand, Result<SourceLinkCategoryDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreateSourceLinkCategoryHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<SourceLinkCategoryDTO>> Handle(CreateSourceLinkCategoryCommand request, CancellationToken cancellationToken)
    {
        var newCategory = _mapper.Map<DAL.Entities.Sources.SourceLinkCategory>(request.newCategory);
        try
        {
            if (newCategory.Title is null)
            {
                const string errorMsg = "Category is null";
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            newCategory = await _repositoryWrapper.SourceCategoryRepository.CreateAsync(newCategory);
            _repositoryWrapper.SaveChanges();

            return Result.Ok(_mapper.Map<SourceLinkCategoryDTO>(newCategory));
        }
        catch (Exception ex)
        {
            string errorMsg = "Failed to create new category";
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }
    }
}