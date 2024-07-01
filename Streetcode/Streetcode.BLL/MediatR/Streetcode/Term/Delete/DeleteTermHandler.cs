using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Delete;

public class DeleteTermHandler : IRequestHandler<DeleteTermCommand, Result<TermDTO>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public DeleteTermHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TermDTO>> Handle(DeleteTermCommand request, CancellationToken cancellationToken)
    {
        var term = await _repository.TermRepository.GetFirstOrDefaultAsync(rt => rt.Title != null && rt.Title.ToLower().Equals(request.Title.ToLower()));

        if (term is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        _repository.TermRepository.Delete(term);

        var resultIsSuccess = await _repository.SaveChangesAsync() > 0;
        var relatedTermDto = _mapper.Map<TermDTO>(term);
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