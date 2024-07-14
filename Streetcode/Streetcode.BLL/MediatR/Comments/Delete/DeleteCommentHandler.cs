using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Comments.Delete
{
    public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result<CommentDTO>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public DeleteCommentHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<CommentDTO>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _repositoryWrapper.CommentRepository.GetFirstOrDefaultAsync(c => c.Id == request.commentId);

            if (comment == null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            await DeleteCommentAsync(comment.Id);

            return Result.Ok(_mapper.Map<CommentDTO>(comment));
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await _repositoryWrapper.CommentRepository
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return;
            }

            var replyIds = comment.Replies.Select(r => r.Id).ToList();

            foreach (var replyId in replyIds)
            {
                await DeleteCommentAsync(replyId);
            }

            _repositoryWrapper.CommentRepository.Delete(comment);
            await _repositoryWrapper.SaveChangesAsync();
        }
    }
}
