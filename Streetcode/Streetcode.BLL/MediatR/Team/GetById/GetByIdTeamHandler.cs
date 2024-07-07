using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Team.GetById
{
    public class GetByIdTeamHandler : IRequestHandler<GetByIdTeamQuery, Result<TeamMemberDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetByIdTeamHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TeamMemberDTO>> Handle(GetByIdTeamQuery request, CancellationToken cancellationToken)
        {
            var team = await _repositoryWrapper
                .TeamRepository
                .GetSingleOrDefaultAsync(
                    predicate: p => p.Id == request.Id,
                    include: x => x.Include(x => x.TeamMemberLinks).Include(x => x.Positions));

            if (team is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<TeamMemberDTO>(team));
        }
    }
}