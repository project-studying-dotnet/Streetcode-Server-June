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

namespace Streetcode.BLL.MediatR.Replies;

public class CreateReplyHandler : IRequestHandler<CreateReplyCommand, Result<ReplyDTO>>
{
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        public CreateReplyHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger, IHttpContextAccessor contextAccessor, ITokenService tokenService)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = contextAccessor;
            _tokenService = tokenService;
        }

        public async Task<Result<ReplyDTO>> Handle(CreateReplyCommand request, CancellationToken cancellationToken)
        {
            var newReply = _mapper.Map<Comment>(request.reply);
            if(newReply is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.CanNotMap, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail<ReplyDTO>(new Error(errorMsg));
            }
            
            var httpContext = _httpContextAccessor.HttpContext;
            if(!httpContext!.Request.Cookies.TryGetValue("accessToken", out var accessToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail<ReplyDTO>(new Error(errorMsg));
            }
            
            if(string.IsNullOrEmpty(accessToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail<ReplyDTO>(new Error(errorMsg));
            }
            
            var userId = _tokenService.GetUserIdFromAccessToken(accessToken!);
            if(userId is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail<ReplyDTO>(new Error(errorMsg));
            }
            
            newReply.UserId = new Guid(userId);
            newReply.CreatedAt = DateTime.UtcNow;
            var reply = await _repositoryWrapper.CommentRepository.CreateAsync(newReply);

            var result = await _repositoryWrapper.SaveChangesAsync() > 0;
            if (result is false)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToCreateA, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail<ReplyDTO>(new Error(errorMsg));
            }
            
            return Result.Ok(_mapper.Map<ReplyDTO>(reply));
        }
}
