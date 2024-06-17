using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create;

public class CreateFactHandler : IRequestHandler<CreateFactCommand, Result<FactDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    public CreateFactHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<FactDto>> Handle(CreateFactCommand request, CancellationToken cancellationToken)
    {
        var newFact = _mapper.Map<DAL.Entities.Streetcode.TextContent.Fact>(request.newFact);
        var repositoryFacts = _repositoryWrapper.FactRepository;

        if (newFact is null)
        {
            const string errorMsg = "New fact cannot be null";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        newFact.ImageId = (newFact.ImageId == 0) ? null : newFact.ImageId;

        if (newFact.StreetcodeId == 0)
        {
            const string errorMsg = "StreetcodeId cannot be 0. Please provide a valid StreetcodeId.";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var entity = await repositoryFacts.CreateAsync(newFact);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (resultIsSuccess)
        {
            return Result.Ok(_mapper.Map<FactDto>(entity));
        }
        else
        {
            const string errorMsg = "Failed to create a fact";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}