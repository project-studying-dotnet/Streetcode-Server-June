using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Create;

public class CreateTermHandler : IRequestHandler<CreateTermCommand, Result<TermDTO>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public CreateTermHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TermDTO>> Handle(CreateTermCommand request, CancellationToken cancellationToken)
    {
        var newTerm = _mapper.Map<Entity>(request.Term);

        if (newTerm is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var existingTerms = await _repository.TermRepository.GetAllAsync(t => t.Title == request.Term.Title);

        if (existingTerms.Any())
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.TermAlreadyExist, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var createdTerm = await _repository.TermRepository.CreateAsync(newTerm);

        try
        {
            var isSuccessResult = await _repository.SaveChangesAsync() > 0;

            if(!isSuccessResult)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.CanNotCreate, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        var createdTermDto = _mapper.Map<TermDTO>(createdTerm);

        if(createdTermDto != null)
        {
            return Result.Ok(createdTermDto);
        }
        else
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}