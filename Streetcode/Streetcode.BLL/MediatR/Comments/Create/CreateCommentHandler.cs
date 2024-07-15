using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Comments;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Comments.Create
{
    public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Result<CommentDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CreateCommentHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger, ITokenService tokenService, IHttpContextAccessor contextAccessor)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
            _tokenService = tokenService;
            _httpContextAccessor = contextAccessor;
        }

        public async Task<Result<CommentDTO>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var newComment = _mapper.Map<Comment>(request.comment);
            if (newComment is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.CanNotMap, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var httpContext = _httpContextAccessor.HttpContext;

            if (!httpContext!.Request.Cookies.TryGetValue("accessToken", out var accessToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var userId = _tokenService.GetUserIdFromAccessToken(accessToken);

            newComment.UserId = new Guid(userId);
            newComment.CreatedAt = DateTime.UtcNow;

            var com = await _repositoryWrapper.CommentRepository.CreateAsync(newComment);

            var resultisSucc = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (resultisSucc)
            {
                return Result.Ok(_mapper.Map<CommentDTO>(com));
            }
            else
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToCreateA, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
