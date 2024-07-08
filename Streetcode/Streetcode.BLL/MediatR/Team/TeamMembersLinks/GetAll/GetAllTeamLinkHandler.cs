using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Team.TeamMembersLinks.GetAll
{
    public class GetAllTeamLinkHandler : IRequestHandler<GetAllTeamLinkQuery, Result<IEnumerable<TeamMemberLinkDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllTeamLinkHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<TeamMemberLinkDTO>>> Handle(GetAllTeamLinkQuery request, CancellationToken cancellationToken)
        {
            var teamLinks = await _repositoryWrapper
                .TeamLinkRepository
                .GetAllAsync();

            if (teamLinks is null)
            {
                string errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<IEnumerable<TeamMemberLinkDTO>>(teamLinks));
        }
    }
}
