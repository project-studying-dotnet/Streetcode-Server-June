using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;

public record GetAllRelatedTermsByTermIdHandler : IRequestHandler<GetAllRelatedTermsByTermIdQuery, Result<IEnumerable<RelatedTermDTO>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;
    private readonly ILoggerService _logger;

    public GetAllRelatedTermsByTermIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
    {
        _mapper = mapper;
        _repository = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<RelatedTermDTO>>> Handle(GetAllRelatedTermsByTermIdQuery request, CancellationToken cancellationToken)
    {
        var relatedTerms = await _repository.RelatedTermRepository
            .GetAllAsync(
                predicate: rt => rt.TermId == request.TermId,
                include: rt => rt.Include(rt => rt.Term));

        if (!relatedTerms.Any())
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request, request.TermId);
            _logger.LogError(request, errorMsg);
            return new Error(errorMsg);
        }

        var relatedTermsDto = _mapper.Map<IEnumerable<RelatedTermDTO>>(relatedTerms);

        if (relatedTermsDto is null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return new Error(errorMsg);
        }

        return Result.Ok(relatedTermsDto);
    }
}