using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Update;

public class UpdateTermHandler : IRequestHandler<UpdateTermCommand, Result<TermDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;
    private readonly ILoggerService _logger;

    public UpdateTermHandler(IMapper mapper, IRepositoryWrapper repository, ILoggerService logger)
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<TermDTO>> Handle(UpdateTermCommand request, CancellationToken cancellationToken)
    {
        var term = await _repository.TermRepository
            .GetFirstOrDefaultAsync(rt => rt.Id == request.Term.Id);

        if (term == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request);
            _logger.LogError(request, errorMsg);
            return new Error(errorMsg);
        }

        var existingTerms = await _repository.TermRepository.GetAllAsync(t => t.Title == request.Term.Title);

        if (existingTerms.Any())
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.TermAlreadyExist, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var termToUpdate = _mapper.Map<Entity>(request.Term);

        if (termToUpdate is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var updatedTerm = _repository.TermRepository.Update(termToUpdate);

        var isSuccessResult = await _repository.SaveChangesAsync() > 0;

        if (!isSuccessResult)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToUpdate, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var updatedTermDto = _mapper.Map<TermDTO>(updatedTerm.Entity);

        if (updatedTermDto != null)
        {
            return Result.Ok(updatedTermDto);
        }
        else
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}