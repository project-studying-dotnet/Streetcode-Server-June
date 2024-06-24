using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Art.Create
{
    public class CreateArtHandler : IRequestHandler<CreateArtCommand, Result<ArtCreateUpdateDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        public CreateArtHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<ArtCreateUpdateDTO>> Handle(CreateArtCommand request, CancellationToken cancellationToken)
        {
            var newArt = _mapper.Map<DAL.Entities.Media.Images.Art>(request.Art);
            if (newArt is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToConvertNull, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var art = await _repositoryWrapper.ArtRepository.CreateAsync(newArt);
            var resultIsSucc = await _repositoryWrapper.SaveChangesAsync() > 0;
            if (resultIsSucc)
            {
                return Result.Ok(_mapper.Map<ArtCreateUpdateDTO>(art));
            }
            else
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToCreateA, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
