using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAll;

public record GetAllRelatedTermsHandler : IRequestHandler<GetAllRelatedTermsQuery, Result<IEnumerable<RelatedTermDTO>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetAllRelatedTermsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repository = repositoryWrapper;
    }

    public async Task<Result<IEnumerable<RelatedTermDTO>>> Handle(GetAllRelatedTermsQuery request, CancellationToken cancellationToken)
    {
        var relatedTerms = await _repository.RelatedTermRepository
            .GetAllAsync(include: rt => rt.Include(rt => rt.Term));

        return Result.Ok(_mapper.Map<IEnumerable<RelatedTermDTO>>(relatedTerms));
    }
}