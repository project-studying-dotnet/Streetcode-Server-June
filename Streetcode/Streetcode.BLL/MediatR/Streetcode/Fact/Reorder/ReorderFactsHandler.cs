using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;

public class ReorderFactsHandler : IRequestHandler<ReorderFactsCommand, Result<IEnumerable<FactDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    public ReorderFactsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<FactDto>>> Handle(ReorderFactsCommand request, CancellationToken cancellationToken)
    {
        var factRepository = _repositoryWrapper.FactRepository;
        var facts = await factRepository.GetAllAsync(f => f.StreetcodeId == request.streetcodeId);
        var listOfNewPosition = request.newPositionsOfFacts;

        if (listOfNewPosition is null || !listOfNewPosition.Any())
        {
            const string errorMsg = $"Updated list of position cannot be empty or null";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (facts is null || !facts.Any())
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFoundWithStreetcode, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        foreach(var fact in listOfNewPosition)
        {
            var newPosition = facts.FirstOrDefault(f => f.Id == fact.Id);

            if(newPosition is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FactNotFoundInListPosition, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            newPosition.Position = fact.NewPosition;
        }

        factRepository.UpdateRange(facts);
        var result = await _repositoryWrapper.SaveChangesAsync() > 0;
        if (!result)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailSaveChangesDB, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<IEnumerable<FactDto>>(facts));
    }
}