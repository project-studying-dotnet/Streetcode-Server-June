using System.Diagnostics;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.GetAll;

public class GetAllFactsHandler : IRequestHandler<GetAllFactsQuery, Result<IEnumerable<FactDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IDistributedCache _distributedCache;

    public GetAllFactsHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger, IDistributedCache distributedCache)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public async Task<Result<IEnumerable<FactDto>>> Handle(GetAllFactsQuery request, CancellationToken cancellationToken)
    {       
        string key = $"FactDTO";

        string? cachedValue = await _distributedCache.GetStringAsync(key, token: cancellationToken);

        // Cache was found
        if (!string.IsNullOrEmpty(cachedValue))
        {            
            List<FactDto> factDto = JsonConvert.DeserializeObject<List<FactDto>>(cachedValue, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            })!;

            if (factDto != null)
            {               
                return Result.Ok(factDto as IEnumerable<FactDto>);
            }
        }

        var facts = await _repositoryWrapper.FactRepository.GetAllAsync();
        if (facts is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(facts), token: cancellationToken);
        
        return Result.Ok(_mapper.Map<IEnumerable<FactDto>>(facts));
    }
}