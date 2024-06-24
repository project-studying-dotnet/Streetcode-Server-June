using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Media.Art.Delete
{
    public class DeleteArtHandler : IRequestHandler<DeleteArtCommand, Result<ArtDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public DeleteArtHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ArtDTO>> Handle(DeleteArtCommand request, CancellationToken cancellationToken)
        {
            var art = await _repositoryWrapper.ArtRepository
                .GetFirstOrDefaultAsync(
                predicate: a => a.Id == request.id,
                include: i => i.Include(a => a.Image));

            if (art == null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request, request.id);
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            try
            {
                _repositoryWrapper.ArtRepository.Delete(art);
                if (art.Image != null)
                {
                    _repositoryWrapper.ImageRepository.Delete(art.Image);
                }
                
                await _repositoryWrapper.SaveChangesAsync();
                return Result.Ok(_mapper.Map<ArtDTO>(art));
            }
            catch (Exception ex)
            {
                _logger.LogError(request, ex.Message);
                return Result.Fail(ex.Message);
            }
        }
    }
}
