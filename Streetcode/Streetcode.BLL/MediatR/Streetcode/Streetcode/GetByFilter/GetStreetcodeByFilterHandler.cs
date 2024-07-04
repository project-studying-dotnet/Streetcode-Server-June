using FluentResults;
using MediatR;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Specification.Media.ArtSpec.GetByStreetcode;
using Streetcode.BLL.Specification.Streetcode.Streetcode.GetByFilter;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Specification.Streetcode.TextContent.FactSpec.GetAll;
using Streetcode.BLL.Specification.TimeLine;
using AutoMapper;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specification.Streetcode.TextSec.GetAll;
using Ardalis.Specification;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;

public class GetStreetcodeByFilterHandler : IRequestHandler<GetStreetcodeByFilterQuery, Result<List<StreetcodeFilterResultDTO>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IMapper _mapper;

    public GetStreetcodeByFilterHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<List<StreetcodeFilterResultDTO>>> Handle(GetStreetcodeByFilterQuery request, CancellationToken cancellationToken)
    {
        string searchQuery = request.Filter.SearchQuery;
        if (string.IsNullOrEmpty(searchQuery))
        {
            string errorMsg = ErrorMessages.EmptyQuery;
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        var results = new List<StreetcodeFilterResultDTO>();

        var streetcodeRepository = _repositoryWrapper.StreetcodeRepository;
        var textRepository = _repositoryWrapper.TextRepository;
        var factRepository = _repositoryWrapper.FactRepository;
        var timelineRepository = _repositoryWrapper.TimelineRepository;
        var artRepository = _repositoryWrapper.ArtRepository;

        await AddResultsAsync(streetcodeRepository, new StreetcodesFilteredByQuerySpec(searchQuery), results);
        await AddResultsAsync(textRepository, new TextFilteredByQuerySpec(searchQuery), results);
        await AddResultsAsync(factRepository, new FactsFilteredByQuerySpec(searchQuery), results);
        await AddResultsAsync(timelineRepository, new TimeLinesIncludePublishStreetcodeSpec(searchQuery), results);
        await AddArtResultsAsync(artRepository, new ArtsFilteredByQuerySpec(searchQuery), results);

        return results;
    }

    private async Task AddResultsAsync<T>(DAL.Repositories.Interfaces.Base.IRepositoryBase<T> repository, Specification<T> spec, List<StreetcodeFilterResultDTO> results)
        where T : class
    {
        var items = await repository.GetAllWithSpecAsync(spec);
        foreach (var item in items!)
        {
            if(item != null)
            {
                results.Add(_mapper.Map<StreetcodeFilterResultDTO>(item));
            }
        }
    }

    private async Task AddArtResultsAsync(IArtRepository repository, Specification<Art> spec, List<StreetcodeFilterResultDTO> results)
    {
        var streetcodeArts = await repository.GetAllWithSpecAsync(spec);

        foreach (var streetcodeArt in streetcodeArts!)
        {
            if (streetcodeArt != null)
            {
                streetcodeArt.StreetcodeArts.ForEach(art =>
                {
                    if (art.Streetcode != null)
                    {
                        results.Add(_mapper.Map<StreetcodeFilterResultDTO>(streetcodeArt));
                    }
                });
            }
        }
    }
}