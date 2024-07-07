using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.Resources;
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

    public async Task<Result<IEnumerable<TextDTO>>> Handle(GetTextByStreetcodeIdQuery request, CancellationToken cancellationToken)
    {
        var texts = await _repositoryWrapper.TextRepository
            .GetAllAsync(text => text.StreetcodeId == request.StreetcodeId);

        if (!texts.Any() &&
            await _repositoryWrapper.StreetcodeRepository
                 .GetFirstOrDefaultAsync(s => s.Id == request.StreetcodeId) == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.StreetcodeNotExist, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        foreach (var text in texts)
        {
            text.TextContent = await _textService.AddTermsTag(text.TextContent ?? "");
        }

        return Result.Ok(_mapper.Map<IEnumerable<TextDTO>>(texts));
    }
}