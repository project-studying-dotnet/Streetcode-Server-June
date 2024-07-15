using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Likes;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Likes;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Likes.PushLike
{
    public class PushLikeHandler : IRequestHandler<PushLikeCommand, Result<LikeDTO>>
    {
        private IRepositoryWrapper _wrapper;
        private ILoggerService _logger;
        private IMapper _mapper;
        private UserManager<User> _userManager;

        public PushLikeHandler(IRepositoryWrapper wrapper, IMapper mapper, ILoggerService logger, UserManager<User> userManager)
        {
            _wrapper = wrapper;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<Result<LikeDTO>> Handle(PushLikeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.pushLike.UserId.ToString());
            var streetcode = await _wrapper.StreetcodeRepository.GetFirstOrDefaultAsync(u => u.Id == request.pushLike.streetcodeId);

            if (user is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var like = _mapper.Map<Like>(request.pushLike);

            if(streetcode is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.StreetcodeNotExist);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var isLikeExist = await _wrapper.LikeRepository.GetFirstOrDefaultAsync(u => u.UserId == request.pushLike.UserId && u.streetcodeId == request.pushLike.streetcodeId);
            if (isLikeExist == null)
            {
                _wrapper.LikeRepository.Create(like);
                streetcode.LikesCount++;
                _wrapper.StreetcodeRepository.Update(streetcode);
            } 
            else
            {
                _wrapper.LikeRepository.Delete(isLikeExist);
                streetcode.LikesCount--;
                _wrapper.StreetcodeRepository.Update(streetcode);
            }

            bool isSuccess = await _wrapper.SaveChangesAsync() > 0;
            if(isSuccess)
            {
                return Result.Ok(_mapper.Map<LikeDTO>(like));
            } 
            else
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToCreateNewLike);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
