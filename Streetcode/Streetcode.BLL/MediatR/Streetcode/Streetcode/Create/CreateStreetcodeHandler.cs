using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Create
{
    public class CreateStreetcodeHandler : IRequestHandler<CreateStreetcodeCommand, Result<CreateStreetcodeDTO>>
    {
        private const int VISIBLETAGS = 10;
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
            var repositoryStreetcodeTagIndex = _repositoryWrapper.StreetcodeTagIndexRepository;
            var repositoryStreetcodeImage = _repositoryWrapper.StreetcodeImageRepository;

            if (newStreetcode is null)
            {
                string errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToConvertNull, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var entity = await repositoryStreetcode.CreateAsync(newStreetcode);
            bool resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (resultIsSuccess)
            {
                List<int> tagIds = request.newStreetcode.TagIds.ToList();
                for (int i = 0; i < tagIds.Count(); i++)
                {
                    StreetcodeTagIndex streetcodeTagIndex = new StreetcodeTagIndex
                    {
                        TagId = tagIds[i],
                        StreetcodeId = entity.Id,
                        Index = entity.Index,
                        IsVisible = i <= VISIBLETAGS
                    };
                    await repositoryStreetcodeTagIndex.CreateAsync(streetcodeTagIndex);
                }

                List<int> imageIds = request.newStreetcode.ImageIds.ToList();
                for (int i = 0; i < imageIds.Count(); i++)
                {
                    StreetcodeImage streetcodeImage = new StreetcodeImage
                    {
                        ImageId = imageIds[i],
                        StreetcodeId = entity.Id
                    };
                    await repositoryStreetcodeImage.CreateAsync(streetcodeImage);
                }

                resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
            }

            if (resultIsSuccess)
            {
                return Result.Ok(_mapper.Map<CreateStreetcodeDTO>(entity));
            }
            else
            {
                string errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToCreateA, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
