using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Newss.SortedByDateTime
{
    public class SortedByDateTimeHandler : IRequestHandler<SortedByDateTimeQuery, Result<List<NewsDTO>>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly IBlobService _blobService;
        private readonly ILoggerService _logger;

        public SortedByDateTimeHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<Result<List<NewsDTO>>> Handle(SortedByDateTimeQuery request, CancellationToken cancellationToken)
        {
            var news = await _repositoryWrapper.NewsRepository.GetAllAsync(
                include: cat => cat.Include(img => img.Image));
            
            if (news is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            news.OrderByDescending(x => x.CreationDate);

            var newsDTOs = _mapper.Map<IEnumerable<NewsDTO>>(news).OrderByDescending(x => x.CreationDate).ToList();

            foreach (var dto in newsDTOs)
            {
                if (dto.Image is not null)
                {
                    dto.Image.Base64 = _blobService.FindFileInStorageAsBase64(dto.Image.BlobName);
                }
            }

            return Result.Ok(newsDTOs);
        }
    }
}
