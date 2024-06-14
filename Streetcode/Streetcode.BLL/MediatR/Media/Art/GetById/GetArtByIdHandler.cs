﻿using System.Net;
using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Exceptions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Art.GetById;

public class GetArtByIdHandler : IRequestHandler<GetArtByIdQuery, Result<ArtDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetArtByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ArtDTO>> Handle(GetArtByIdQuery request, CancellationToken cancellationToken)
    {
        var art = await _repositoryWrapper.ArtRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id);

        if (art is null)
        {
            var errorMsg = $"Cannot find an art with corresponding id: {request.Id}";
            _logger.LogError(request, errorMsg);
            throw new RequestException(errorMsg, HttpStatusCode.NotFound);
        }

        return Result.Ok(_mapper.Map<ArtDTO>(art));
    }
}