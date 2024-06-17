using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Feedback;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Realizations.AdditionalContent;

namespace Streetcode.BLL.MediatR.Streetcode.Facts.Update
{
    public class UpdateFactHandler : IRequestHandler<UpdateFactCommand, Result<FactDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        public UpdateFactHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper,  ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<FactDto>> Handle(UpdateFactCommand request, CancellationToken cancellationToken)
        {
            var fact = _mapper.Map<DAL.Entities.Streetcode.TextContent.Fact>(request.Fact);
            if (fact is null)
            {
                const string errorMsg = $"Cannot convert FactDto to Fact Entity";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repositoryWrapper.FactRepository.Update(fact);
            var resItIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
            if (resItIsSuccess)
            {
                return Result.Ok(_mapper.Map<FactDto>(fact));
            }
            else
            {
                const string errorMsg = $"Failed to update fact";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
