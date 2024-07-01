using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;

public class UpdateRelatedTermHandler : IRequestHandler<UpdateRelatedTermCommand, Result<RelatedTermDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;
    private readonly ILoggerService _logger;

    public UpdateRelatedTermHandler(IMapper mapper, IRepositoryWrapper repository, ILoggerService logger)
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<RelatedTermDTO>> Handle(UpdateRelatedTermCommand request, CancellationToken cancellationToken)
    {
        var relatedTerm = await _repository.RelatedTermRepository
            .GetFirstOrDefaultAsync(rt => rt.Id == request.RelatedTerm.Id);

        if (relatedTerm == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request);
            _logger.LogError(request, errorMsg);
            return new Error(errorMsg);
        }

        var existingTerms = await _repository.RelatedTermRepository.GetAllAsync(t => t.Word!.ToLower().Equals(request.RelatedTerm.Word.ToLower()));

        if (existingTerms.Any())
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.TermAlreadyExist, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }


        var relatedTermToUpdate = _mapper.Map<Entity>(request.RelatedTerm);

        if (relatedTermToUpdate is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var updatedRelatedTerm = _repository.RelatedTermRepository.Update(relatedTermToUpdate);

        var isSuccessResult = await _repository.SaveChangesAsync() > 0;

        if (!isSuccessResult)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToUpdate, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var updatedRelatedTermDto = _mapper.Map<RelatedTermDTO>(updatedRelatedTerm.Entity);

        if (updatedRelatedTermDto != null)
        {
            return Result.Ok(updatedRelatedTermDto);
        }
        else
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}