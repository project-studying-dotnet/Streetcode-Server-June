using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Update;

public class UpdateTextHandler : IRequestHandler<UpdateTextCommand, Result<TextDTO>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IMapper _mapper;

    public UpdateTextHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger, IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<TextDTO>> Handle(UpdateTextCommand request, CancellationToken cancellationToken)
    {
        var text = await _repositoryWrapper.TextRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id);

        if (text is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var newText = _mapper.Map<Entity>(request.TextUpdate);
        newText.Id = request.Id;

        var updatedText = _repositoryWrapper.TextRepository.Update(newText);

        var isSuccessResult = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSuccessResult)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToUpdate, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var updatedTextDto = _mapper.Map<TextDTO>(updatedText.Entity);

        if (updatedTextDto != null)
        {
            return Result.Ok(updatedTextDto);
        }
        else
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, updatedText.Entity);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}