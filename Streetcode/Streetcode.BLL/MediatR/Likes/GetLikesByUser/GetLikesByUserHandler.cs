using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Likes;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Likes;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Likes.PushLike
{
    public class GetLikesByUserHandler : IRequestHandler<GetLikesByUserQuery, Result<IEnumerable<StreetcodeDTO>>>
    {
        private IRepositoryWrapper _wrapper;
        private ILoggerService _logger;
        private IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenService _tokenService;
        private UserManager<User> _userManager;

        public GetLikesByUserHandler(
            IRepositoryWrapper wrapper,
            IMapper mapper, 
            ILoggerService logger,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService,
            UserManager<User> userManager)
        {
            _wrapper = wrapper;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<Result<IEnumerable<StreetcodeDTO>>> Handle(GetLikesByUserQuery request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (!httpContext!.Request.Cookies.TryGetValue("accessToken", out var accessToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var userId = _tokenService.GetUserIdFromAccessToken(accessToken);
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var likes = await _wrapper.LikeRepository.GetAllAsync(
                 predicate: u => u.UserId == user.Id,
                 include: q => q.Include(s => s.Streetcode));

            var streetcodes = likes.Select(likeDTO => likeDTO.Streetcode).Distinct();

            return Result.Ok(_mapper.Map<IEnumerable<StreetcodeDTO>>(streetcodes));
        }
    }
}