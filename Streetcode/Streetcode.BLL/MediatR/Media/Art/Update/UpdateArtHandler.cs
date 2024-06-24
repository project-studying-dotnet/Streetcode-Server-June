using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Media.Art.Update
{
    public class UpdateArtHandler : IRequestHandler<UpdateArtCommand, Result<ArtCreateUpdateDTO>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        public UpdateArtHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ArtCreateUpdateDTO>> Handle(UpdateArtCommand request, CancellationToken cancellationToken)
        {
            var art = _mapper.Map<DAL.Entities.Media.Images.Art>(request.ArtDto);
            if (art == null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToConvertNull, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repositoryWrapper.ArtRepository.Update(art);
            var resIsSucc = await _repositoryWrapper.SaveChangesAsync() > 0;
            if (resIsSucc)
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
