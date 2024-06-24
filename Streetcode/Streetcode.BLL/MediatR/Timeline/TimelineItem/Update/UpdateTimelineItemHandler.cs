using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.BLL.Exceptions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update
{
    public class UpdateTimelineItemHandler : IRequestHandler<UpdateTimelineItemCommand, Result<TimelineItemDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public UpdateTimelineItemHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TimelineItemDTO>> Handle(UpdateTimelineItemCommand request, CancellationToken cancellationToken)
        {
            var sourceTimelineItem = request.sourceTimeLine;
            var updatingTimelineItem = await _repositoryWrapper.TimelineRepository
                .GetFirstOrDefaultAsync(
                predicate: t => t.Id == sourceTimelineItem.Id,
                include: i => i.Include(t => t.HistoricalContextTimelines));

            if (updatingTimelineItem == null)
            {
                string errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request, request.sourceTimeLine.Id);
                _logger.LogError(request, errorMsg);
                return Result.Fail<TimelineItemDTO>(errorMsg);
            }

            try
            {
                var historicalContextTitles = sourceTimelineItem.HistoricalContexts!.Select(hc => hc.Title).ToList();
                var existingHistoricalContexts = await _repositoryWrapper.HistoricalContextRepository.GetAllAsync(
                    hc => historicalContextTitles.Contains(hc.Title!));
                var newHistoricalContexts = historicalContextTitles
                    .Except(existingHistoricalContexts.Select(hc => hc.Title))
                    .Select(title => new DAL.Entities.Timeline.HistoricalContext { Title = title })
                    .ToList();

                if (newHistoricalContexts.Any())
                {
                    await _repositoryWrapper.HistoricalContextRepository.CreateRangeAsync(newHistoricalContexts);
                    await _repositoryWrapper.SaveChangesAsync();
                }

                var allHistoricalContexts = existingHistoricalContexts.Concat(newHistoricalContexts).ToList();

                var historicalContextTimelines = allHistoricalContexts.Select(hc => new HistoricalContextTimeline
                {
                    HistoricalContextId = hc.Id,
                    HistoricalContext = hc,
                    TimelineId = updatingTimelineItem.Id,
                    Timeline = updatingTimelineItem
                }).ToList();

                updatingTimelineItem.Title = sourceTimelineItem.Title;
                updatingTimelineItem.Description = sourceTimelineItem.Description;
                updatingTimelineItem.DateViewPattern = sourceTimelineItem.DateViewPattern;
                updatingTimelineItem.Date = sourceTimelineItem.Date;
                
                _repositoryWrapper.TimelineRepository.Update(updatingTimelineItem);

                updatingTimelineItem.HistoricalContextTimelines.Clear();
                updatingTimelineItem.HistoricalContextTimelines = historicalContextTimelines;
                
                await _repositoryWrapper.SaveChangesAsync();

                return Result.Ok(_mapper.Map<TimelineItemDTO>(updatingTimelineItem));
            }
            catch (Exception ex)
            {
                _logger.LogError(request, ex.Message);
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
