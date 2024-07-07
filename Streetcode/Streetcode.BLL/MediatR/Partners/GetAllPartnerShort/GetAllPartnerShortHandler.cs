using AutoMapper;
using FluentResults;
using MediatR;
using Newtonsoft.Json;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Partners.GetAllPartnerShort
{
    internal class GetAllPartnerShortHandler : IRequestHandler<GetAllPartnersShortQuery, Result<IEnumerable<PartnerShortDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllPartnerShortHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<PartnerShortDTO>>> Handle(GetAllPartnersShortQuery request, CancellationToken cancellationToken)
        {
            if (request.CachedResponse?.IsSuccess == true)
            {
                return request.CachedResponse;
            }

            var partners = await _repositoryWrapper.PartnersRepository.GetAllAsync();

            if (partners is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            return Result.Ok(_mapper.Map<IEnumerable<PartnerShortDTO>>(partners));
        }
    }
}
