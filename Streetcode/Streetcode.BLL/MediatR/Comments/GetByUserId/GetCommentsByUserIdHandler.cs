using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specification.Comment;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Comments.GetByUserId
{
    public class GetCommentsByUserIdHandler : IRequestHandler<GetCommentsByUserIdQuery, Result<IEnumerable<CommentDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetCommentsByUserIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<CommentDTO>>> Handle(GetCommentsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var comments = await _repositoryWrapper
               .CommentRepository.GetAllWithSpecAsync(new CommentsByUserIdSpec(request.UserId));

            return Result.Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
        }
    }
}
