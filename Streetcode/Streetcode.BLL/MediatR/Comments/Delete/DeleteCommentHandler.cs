using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Security.Claims;

namespace Streetcode.BLL.MediatR.Comments.Delete
{
    public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result<CommentDTO>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteCommentHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = contextAccessor;
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

            if (!(GetUserIdFromHttpContext() == comment.UserId))
            {
                if (!_httpContextAccessor.HttpContext.User.IsInRole(UserRole.Admin.ToString()))
                {
                    var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request);
                    _logger.LogError(request, errorMsg);
                    return Result.Fail(new Error(errorMsg));
                }
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

        private Guid? GetUserIdFromHttpContext()
        {
            var userIdString = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString != null && Guid.TryParse(userIdString, out var userId))
            {
                return userId;
            }

            return Guid.Empty;
        }
    }
}
