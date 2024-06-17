using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;


namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete
{
    public class DeleteTimelineItemHandler : IRequestHandler<DeleteTimelineItemCommand, Result<Unit>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public DeleteTimelineItemHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(DeleteTimelineItemCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;
            var timeline = await _repositoryWrapper.TimelineRepository.GetFirstOrDefaultAsync(x => x.Id == id);

            string errorMsg;
            if (timeline is null)
            {
                errorMsg = "Cannot find timeline item";
                _logger.LogError(request, errorMsg);

                return Result.Fail(errorMsg);
            }

            _repositoryWrapper.TimelineRepository.Delete(timeline);

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (resultIsSuccess)
            {
                return Result.Ok(Unit.Value);
            }

            errorMsg = "Cannot delete timeline item";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}