using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Likes;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Likes;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Likes.PushLike
{
    public class GetLikesByUserId : IRequestHandler<GetLikesByUserIdQuery, Result<IEnumerable<LikeDTO>>>
    {
        private IRepositoryWrapper _wrapper;
        private ILoggerService _logger;
        private IMapper _mapper;
        private UserManager<User> _userManager;

        public GetLikesByUserId(IRepositoryWrapper wrapper, IMapper mapper, ILoggerService logger, UserManager<User> userManager)
        {
            _wrapper = wrapper;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<Result<IEnumerable<LikeDTO>>> Handle(GetLikesByUserIdQuery request, CancellationToken cancellationToken)
        {
            var user = _userManager.FindByIdAsync(request.userId.ToString());

            if (user is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var likes = _wrapper.LikeRepository.GetAllAsync(s => s.UserId == request.userId);

            return Result.Ok(_mapper.Map<IEnumerable<LikeDTO>>(likes));
        }
    }
}
