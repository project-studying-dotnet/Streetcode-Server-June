using AutoMapper;
using FluentResults;
using MediatR;
using NLog.Targets;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedFigure.Create;

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
        var newCategory = _mapper.Map<SourceLinkCategory>(request.newCategory);
        _repositoryWrapper.SourceCategoryRepository.Create(newCategory);

        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
        if(resultIsSuccess)
        {
            return Result.Ok(_mapper.Map<SourceLinkCategoryDTO>(newCategory));
        }
        else
        {
            string errorMsg = "Failed to create new category";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}
