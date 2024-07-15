using System.Security.Claims;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
namespace Streetcode.BLL.MediatR.Comments.Update;

public class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand, Result<CommentDTO>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _loggerService;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateCommentHandler(IRepositoryWrapper repositoryWrapper, ILoggerService loggerService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _repositoryWrapper = repositoryWrapper;
        _loggerService = loggerService;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<CommentDTO>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var editedComment = request.EditedComment;
        var commentRepository = _repositoryWrapper.CommentRepository;

        var userId = GetUserIdFromHttpContext();
        if (userId == Guid.Empty)
        {
            var errorMsg = ErrorMessages.UserNotAuthenticated;
            _loggerService.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var comment = await commentRepository.GetFirstOrDefaultAsync(cm => cm.Id == editedComment.Id);
        if (comment == null)
        {
           var errorMsg = string.Format(ErrorMessages.CommentNotFound, editedComment.Id);
           _loggerService.LogError(request, errorMsg);
           return Result.Fail(new Error(errorMsg));
        }

        if (comment.UserId != userId)
        {
            var errorMsg = string.Format(ErrorMessages.UnauthorizedAccessForEditComment, userId);
            _loggerService.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        comment.CommentContent = editedComment.CommentContent;
        comment.EditedAt = DateTime.UtcNow;

        var updatedComment = commentRepository.Update(comment);

        if (updatedComment == null || updatedComment.Entity == null)
        {
            var errorMsg = string.Format(ErrorMessages.FailToUpdateComment, comment.Id);
            _loggerService.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (await _repositoryWrapper.SaveChangesAsync() <= 0)
        {
            var errorMsg = string.Format(ErrorMessages.FailSaveChangesDB);
            _loggerService.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var commentDTO = _mapper.Map<CommentDTO>(updatedComment.Entity);
        if (commentDTO == null)
        {
            var errorMsg = string.Format(ErrorMessages.FailToMap);
            _loggerService.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(commentDTO);
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
