using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.GetAll;

public record GetAllTermsQuery : IRequest<Result<IEnumerable<TermDTO>>>;