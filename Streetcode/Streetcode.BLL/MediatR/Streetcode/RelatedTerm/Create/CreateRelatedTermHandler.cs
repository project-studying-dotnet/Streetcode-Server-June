using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;

public class CreateRelatedTermHandler : IRequestHandler<CreateRelatedTermCommand, Result<RelatedTermDTO>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public CreateRelatedTermHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<RelatedTermDTO>> Handle(CreateRelatedTermCommand request, CancellationToken cancellationToken)
    {
        var relatedTerm = _mapper.Map<Entity>(request.RelatedTerm);

        if (relatedTerm is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var existingTerms = await _repository.RelatedTermRepository
            .GetAllAsync(
                predicate: rt => rt.Word != null && rt.TermId == request.RelatedTerm.TermId && rt.Word.ToLower().Equals(request.RelatedTerm.Word.ToLower()));

        if (existingTerms.Any())
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.TermAlreadyExist, request.RelatedTerm);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var createdRelatedTerm = await _repository.RelatedTermRepository.CreateAsync(relatedTerm);

        var isSuccessResult = await _repository.SaveChangesAsync() > 0;

        if (!isSuccessResult)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.CanNotCreate, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var createdRelatedTermDto = _mapper.Map<RelatedTermDTO>(createdRelatedTerm);

        if(createdRelatedTermDto != null)
        {
            return Result.Ok(createdRelatedTermDto);
        }
        else
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}