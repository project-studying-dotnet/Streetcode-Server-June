using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.WebApi.Controllers.Comment;

namespace Streetcode.BLL.MediatR.Comments.GetAllByStreetcodeId
{
    public class GetAllCommentsByStreetcodeIdHandler : IRequestHandler<GetAllCommentsByStreetcodeIdQuery, Result<IEnumerable<CommentDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllCommentsByStreetcodeIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<CommentDTO>>> Handle(GetAllCommentsByStreetcodeIdQuery request, CancellationToken cancellationToken)
        {
            var comments = await _repositoryWrapper.CommentRepository
                .GetAllAsync(c => c.StreetcodeId == request.streetcodeId);

            if (comments is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFoundWithStreetcode, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
        }
    }
}
