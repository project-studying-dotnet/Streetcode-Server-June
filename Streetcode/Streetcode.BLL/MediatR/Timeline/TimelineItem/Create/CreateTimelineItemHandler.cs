using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;


namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Create
{
    public class CreateTimelineItemHandler : IRequestHandler<CreateTimelineItemCommand, Result<CreateTimelineItemDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public CreateTimelineItemHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CreateTimelineItemDTO>> Handle(CreateTimelineItemCommand request, CancellationToken cancellationToken)
        {
            var newTimelineItem = _mapper.Map<DAL.Entities.Timeline.TimelineItem>(request.newTimeLine);

            if (string.IsNullOrEmpty(newTimelineItem?.Title))
            {
                string errorMsg = MessageResourceContext.GetMessage(ErrorMessages.TitleNullEmpty, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail<CreateTimelineItemDTO>(errorMsg);
            }

            try
            {
                newTimelineItem.HistoricalContextTimelines.Clear();
                await _repositoryWrapper.TimelineRepository.CreateAsync(newTimelineItem);
                await _repositoryWrapper.SaveChangesAsync();

                var historicalContextTitles = request.newTimeLine.HistoricalContexts!.Select(hc => hc.Title).ToList();
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
                    TimelineId = newTimelineItem.Id,
                    Timeline = newTimelineItem
                }).ToList();

                newTimelineItem.HistoricalContextTimelines.AddRange(historicalContextTimelines);
                await _repositoryWrapper.SaveChangesAsync();

                return Result.Ok(_mapper.Map<CreateTimelineItemDTO>(newTimelineItem));
            }
            catch (Exception ex)
            {
                _logger.LogError(request, ex.Message);
                return Result.Fail<CreateTimelineItemDTO>(ex.Message);
            }
        }
    }
}
