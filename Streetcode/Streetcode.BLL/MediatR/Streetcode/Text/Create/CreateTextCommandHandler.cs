using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Create;

public class CreateTextCommandHandler : IRequestHandler<CreateTextCommand, Result<TextDTO>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public CreateTextCommandHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TextDTO>> Handle(CreateTextCommand request, CancellationToken cancellationToken)
    {
        var newText = _mapper.Map<Entity>(request.TextCreate);

        if (newText is null)
        {
            const string errorMsg = "Cannot create new Text entity!";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var createdText = await _repository.TextRepository.CreateAsync(newText);
        
        var isSuccessResult = await _repository.SaveChangesAsync() > 0;
        
        if(!isSuccessResult)
        {
            const string errorMsg = "Cannot save changes in the database after Text creation!";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var createdTextDto = _mapper.Map<TextDTO>(createdText);

        if(createdTextDto != null)
        {
            return Result.Ok(createdTextDto);
        }
        else
        {
            const string errorMsg = "Cannot map entity!";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}