using Ardalis.Specification;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specification.Streetcode.Streetcode.GetAll;
using Streetcode.BLL.Specification.Streetcode.Streetcode.GetAllCatalog;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog
{
    public class GetAllStreetcodesCatalogHandler : IRequestHandler<GetAllStreetcodesCatalogQuery,
          Result<IEnumerable<RelatedFigureDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllStreetcodesCatalogHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<RelatedFigureDTO>>> Handle(GetAllStreetcodesCatalogQuery request, CancellationToken cancellationToken)
        {
            List<ISpecification<StreetcodeContent>> specifications = new List<ISpecification<StreetcodeContent>>();
            specifications.Add(new StreetcodesInclTagsImagesSpec());
            specifications.Add(new StreetcodeApplyPaginationSpec(request.count, request.page));

            var streetcodes = await _repositoryWrapper.StreetcodeRepository.GetAllWithSpecAsync(specifications.ToArray());

            if (streetcodes != null)
            {
                return Result.Ok(_mapper.Map<IEnumerable<RelatedFigureDTO>>(streetcodes));
            }

            string errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }
    }
}
