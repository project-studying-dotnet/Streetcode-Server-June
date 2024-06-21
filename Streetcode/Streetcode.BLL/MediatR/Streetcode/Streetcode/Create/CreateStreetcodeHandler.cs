using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Util;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Create
{
    public class CreateStreetcodeHandler : IRequestHandler<CreateStreetcodeCommand, Result<CreateStreetcodeDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public CreateStreetcodeHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CreateStreetcodeDTO>> Handle(CreateStreetcodeCommand request, CancellationToken cancellationToken)
        {
            var newStreetcode = _mapper.Map<StreetcodeContent>(request.newStreetcode);
            var repositoryStreetcode = _repositoryWrapper.StreetcodeRepository;

            if (newStreetcode is null)
            {
                const string errorMsg = "New Streetcode cannot be null";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            // ..

            var entity = await repositoryStreetcode.CreateAsync(newStreetcode);
            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (resultIsSuccess)
            {
                return Result.Ok(_mapper.Map<CreateStreetcodeDTO>(entity));
            }
            else
            {
                const string errorMsg = "Failed to create a Streetcode";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
