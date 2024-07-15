using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Interfaces.BlobStorage;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId;

public class GetRelatedFiguresByStreetcodeIdHandler : IRequestHandler<GetRelatedFigureByStreetcodeIdQuery, Result<IEnumerable<RelatedFigureDTO>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IBlobService _blobService;

    public GetRelatedFiguresByStreetcodeIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger, IBlobService blobService)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _blobService = blobService;
    }

    public async Task<Result<IEnumerable<RelatedFigureDTO>>> Handle(GetRelatedFigureByStreetcodeIdQuery request, CancellationToken cancellationToken)
    {
        var relatedFigureIds = GetRelatedFigureIdsByStreetcodeId(request.StreetcodeId);

        if (!relatedFigureIds.Any())
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFoundWithStreetcode, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var relatedFigures = await _repositoryWrapper.StreetcodeRepository.GetAllAsync(
          predicate: sc => relatedFigureIds.Any(id => id == sc.Id) && sc.Status == DAL.Enums.StreetcodeStatus.Published,
          include: scl => scl.Include(sc => sc.Images).ThenInclude(img => img.ImageDetails) 
                             .Include(sc => sc.Tags));

        if (!relatedFigures.Any())
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithStreetcodeNotFound, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        foreach(StreetcodeContent streetcode in relatedFigures)
        {
            if(streetcode.Images.Any())
            {
                streetcode.Images = streetcode.Images.OrderBy(img => img.ImageDetails?.Alt).ToList();
            }
        }

        var relatedFiguresDto = _mapper.Map<IEnumerable<RelatedFigureDTO>>(relatedFigures);

        if (relatedFiguresDto == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        foreach (var figure in relatedFiguresDto)
        {
            figure.CurrentStreetcodeId = request.StreetcodeId;
            if (figure.Images != null && figure.Images.Any())
            {
                foreach (var image in figure.Images)
                {
                    image.Base64 = _blobService.FindFileInStorageAsBase64(image.BlobName);
                }
            }
        }

        return Result.Ok(relatedFiguresDto);
    }

    private List<int> GetRelatedFigureIdsByStreetcodeId(int StreetcodeId)
    {
        var observerIds = _repositoryWrapper.RelatedFigureRepository
            .FindAll(f => f.TargetId == StreetcodeId).Select(o => o.ObserverId);

        var targetIds = _repositoryWrapper.RelatedFigureRepository
            .FindAll(f => f.ObserverId == StreetcodeId).Select(t => t.TargetId);

        return observerIds.Union(targetIds).Distinct().ToList();
    }
}