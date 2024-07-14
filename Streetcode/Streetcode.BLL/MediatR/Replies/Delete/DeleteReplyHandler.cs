using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Comments;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Replies.Delete
{
    public class DeleteReplyHandler : IRequestHandler<DeleteReplyCommand, Result<CommentDTO>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public DeleteReplyHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<CommentDTO>> Handle(DeleteReplyCommand request, CancellationToken cancellationToken)
        {
            var reply = await _repositoryWrapper.CommentRepository.GetFirstOrDefaultAsync(
                selector: c => c,
                predicate: f => f.Id == request.Id,
                include: q => q.Include(c => c.Replies));

            if (reply == null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            try
            {
                _repositoryWrapper.CommentRepository.DeleteRange(reply.Replies);
                _repositoryWrapper.CommentRepository.Delete(reply);

                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
                if (resultIsSuccess)
                {
                    return Result.Ok(_mapper.Map<CommentDTO>(reply));
                }
                else
                {
                    var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToDeleteA, request);
                    _logger.LogError(request, errorMsg);
                    return Result.Fail(new Error(errorMsg));
                }
            }
            catch (Exception ex)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToDeleteA, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
