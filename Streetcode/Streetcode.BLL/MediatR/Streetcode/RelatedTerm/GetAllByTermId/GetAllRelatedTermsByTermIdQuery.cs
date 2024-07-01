using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;

public record GetAllRelatedTermsByTermIdQuery(int TermId) : IRequest<Result<IEnumerable<RelatedTermDTO>>>;