﻿using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.DTO.Transactions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.MediatR.ResultVariations;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;

public class GetTextByStreetcodeIdHandler : IRequestHandler<GetTextByStreetcodeIdQuery, Result<IEnumerable<TextDTO>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ITextService _textService;
    private readonly ILoggerService _logger;

    public GetTextByStreetcodeIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ITextService textService, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _textService = textService;
        _logger = logger;
    }

    // TODO: should be updated to return all TextDTO by StreetcodeId ???
    public async Task<Result<IEnumerable<TextDTO>>> Handle(GetTextByStreetcodeIdQuery request, CancellationToken cancellationToken)
    {
        var texts = await _repositoryWrapper.TextRepository
            .GetAllAsync(text => text.StreetcodeId == request.StreetcodeId);

        var enumerable = texts.ToList();

        if (!enumerable.Any())
        {
            if (await _repositoryWrapper.StreetcodeRepository
                 .GetFirstOrDefaultAsync(s => s.Id == request.StreetcodeId) == null)
            {
                var errorMsg = $"Cannot find a transaction link by a streetcode id: {request.StreetcodeId}, because such streetcode doesn`t exist";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }

        foreach (var text in enumerable)
        {
            text.TextContent = await _textService.AddTermsTag(text.TextContent ?? "");
        }

        return _mapper.Map<Result<IEnumerable<TextDTO>>>(enumerable);
    }
}