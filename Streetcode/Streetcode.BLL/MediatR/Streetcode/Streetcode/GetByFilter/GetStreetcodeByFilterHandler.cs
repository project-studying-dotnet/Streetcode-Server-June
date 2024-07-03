using FluentResults;
using MediatR;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Specification.Media.ArtSpec.GetByStreetcode;
using Streetcode.BLL.Specification.Streetcode.Streetcode.GetByFilter;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Streetcode.BLL.Specification.Streetcode.TextContent.FactSpec.GetAll;
using Streetcode.BLL.Specification.TimeLine;
using AutoMapper;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specification.Streetcode.TextSec.GetAll;
using Streetcode.DAL.Entities.Timeline;

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
        var results = new List<StreetcodeFilterResultDTO>();

        if (string.IsNullOrEmpty(searchQuery)) 
        {
            string errorMsg = ErrorMessages.EmptyQuery;
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        var streetcodeRepository = _repositoryWrapper.StreetcodeRepository;
        var textRepository = _repositoryWrapper.TextRepository;
        var factRepository = _repositoryWrapper.FactRepository;
        var timelineRepository = _repositoryWrapper.TimelineRepository;
        var artRepository = _repositoryWrapper.ArtRepository;

        await AddStreetcodeResultsAsync(streetcodeRepository, searchQuery, results);
        await AddTextResultsAsync(textRepository, searchQuery, results);
        await AddFactResultsAsync(factRepository, searchQuery, results);
        await AddTimelineResultsAsync(timelineRepository, searchQuery, results);
        await AddArtResultsAsync(artRepository, searchQuery, results);

        return results;
    }

    private async Task AddStreetcodeResultsAsync(IStreetcodeRepository repository, string searchQuery, List<StreetcodeFilterResultDTO> results)
    {
        var streetcodes = await repository.GetAllWithSpecAsync(new StreetcodesFilteredByQuerySpec(searchQuery));

        foreach (var streetcode in streetcodes!)
        {
           results.Add(_mapper.Map<StreetcodeFilterResultDTO>(streetcode));
        }
    }

    private async Task AddTextResultsAsync(ITextRepository repository, string searchQuery, List<StreetcodeFilterResultDTO> results)
    {
        var texts = await repository.GetAllWithSpecAsync(new TextFilteredByQuerySpec(searchQuery));

        foreach (var text in texts!)
        {
            results.Add(_mapper.Map<StreetcodeFilterResultDTO>(text));
        }
    }

    private async Task AddFactResultsAsync(IFactRepository repository, string searchQuery, List<StreetcodeFilterResultDTO> results)
    {
        var facts = await repository.GetAllWithSpecAsync(new FactsFilteredByQuerySpec(searchQuery));

        foreach (var fact in facts!)
        {
            results.Add(_mapper.Map<StreetcodeFilterResultDTO>(fact));
        }
    }

    private async Task AddTimelineResultsAsync(ITimelineRepository repository, string searchQuery, List<StreetcodeFilterResultDTO> results)
    {
        var timelineItems = await repository.GetAllWithSpecAsync(new TimeLinesIncludePublishStreetcodeSpec(searchQuery));

        foreach (var timelineItem in timelineItems!)
        {
            results.Add(_mapper.Map<StreetcodeFilterResultDTO>(timelineItem));
        }
    }

    private async Task AddArtResultsAsync(IArtRepository repository, string searchQuery, List<StreetcodeFilterResultDTO> results)
    {
        var streetcodeArts = await repository.GetAllWithSpecAsync(new ArtsFilteredByQuerySpec(searchQuery));

        foreach (var streetcodeArt in streetcodeArts!)
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