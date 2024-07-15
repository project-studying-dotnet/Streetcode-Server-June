using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Comments.GetAll
{
    public class GetAllCommentsHandler : IRequestHandler<GetAllCommentsQuery, Result<IEnumerable<CommentDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllCommentsHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<CommentDTO>>> Handle(GetAllCommentsQuery request, CancellationToken cancellationToken)
        {
            var comments = await _repositoryWrapper
                .CommentRepository
                .GetAllAsync();

            if (comments is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            return Result.Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
        }
    }
}
