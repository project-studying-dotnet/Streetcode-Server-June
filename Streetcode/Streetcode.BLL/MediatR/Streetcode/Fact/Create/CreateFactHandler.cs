using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create;

public class CreateFactHandler : IRequestHandler<CreateFactCommand, Result<FactDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IDistributedCache _distributedCache;

    public CreateFactHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger, IDistributedCache distrCache)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _distributedCache = distrCache;
    }

    public async Task<Result<FactDto>> Handle(CreateFactCommand request, CancellationToken cancellationToken)
    {
        string key = $"FactDTO";

        var newFact = _mapper.Map<DAL.Entities.Streetcode.TextContent.Fact>(request.NewFact);
        var repositoryFacts = _repositoryWrapper.FactRepository;

        if (newFact is null)
        {
            const string errorMsg = "New fact cannot be null";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        newFact.ImageId = (newFact.ImageId == 0) ? null : newFact.ImageId;

        if (newFact.StreetcodeId == 0)
        {
            const string errorMsg = "StreetcodeId cannot be 0. Please provide a valid StreetcodeId.";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var entity = await repositoryFacts.CreateAsync(newFact);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (resultIsSuccess)
        {
            var old = await _distributedCache.GetStringAsync(key);
            var list = JsonConvert.DeserializeObject<List<DAL.Entities.Streetcode.TextContent.Fact>>(old!);
            if (!list!.Any(s => s.Id == entity.Id))
            {
                list!.Add(newFact);
            }
            
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(list), token: cancellationToken);
           
            return Result.Ok(_mapper.Map<FactDto>(entity));
        }
        else
        {
            const string errorMsg = "Failed to create a fact";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}