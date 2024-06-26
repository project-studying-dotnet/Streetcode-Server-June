﻿using AutoMapper;
using FluentResults;
using MediatR;
using Newtonsoft.Json;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;

public class GetFactByStreetcodeIdHandler : IRequestHandler<GetFactByStreetcodeIdQuery, Result<IEnumerable<FactDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetFactByStreetcodeIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<FactDto>>> Handle(GetFactByStreetcodeIdQuery request, CancellationToken cancellationToken)
    {
        if(request.CachedResponse is not null)
        {
            var cachedFactDto = JsonConvert.DeserializeObject<IEnumerable<FactDto>>(request.CachedResponse.ToString() !);
            return Result.Ok(cachedFactDto) !;
        }
        
        var fact = await _repositoryWrapper.FactRepository
            .GetAllAsync(f => f.StreetcodeId == request.StreetcodeId);

        if (fact is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFoundWithStreetcode, request, request.StreetcodeId);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<IEnumerable<FactDto>>(fact));
    }
}