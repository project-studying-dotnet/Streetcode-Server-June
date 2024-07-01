using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.DeleteById;

public class DeleteRelatedTermByIdHandler : IRequestHandler<DeleteRelatedTermByIdCommand, Result<RelatedTermDTO>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public DeleteRelatedTermByIdHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<RelatedTermDTO>> Handle(DeleteRelatedTermByIdCommand request, CancellationToken cancellationToken)
    {
        var relatedTerm = await _repository.RelatedTermRepository.GetFirstOrDefaultAsync(rt => rt.Id == request.Id);

        if (relatedTerm is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        _repository.RelatedTermRepository.Delete(relatedTerm);

        var resultIsSuccess = await _repository.SaveChangesAsync() > 0;
        var relatedTermDto = _mapper.Map<RelatedTermDTO>(relatedTerm);
        if(resultIsSuccess && relatedTermDto != null)
        {
            return Result.Ok(relatedTermDto);
        }
        else
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToDeleteA, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}