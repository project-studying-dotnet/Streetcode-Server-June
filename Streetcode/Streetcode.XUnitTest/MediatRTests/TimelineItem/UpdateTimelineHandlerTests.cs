using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem
{
    public class UpdateTimelineHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private UpdateTimelineItemHandler _handler;

        public UpdateTimelineHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_TimelineItemNotFound_ThrowsRequestException()
        {
            // Arrange
            var command = new UpdateTimelineItemCommand(new TimelineItemDTO { Id = 1 });
            _mapperMock.Setup(m => m.Map<TimelineItemEntity>(It.IsAny<TimelineItemDTO>())).Returns(new TimelineItemEntity { Id = 1 });
            MockRepositoryWrapper(returnNull: true);

            _handler = new UpdateTimelineItemHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            var result = await _handler.Handle(command, CancellationToken.None);

            // Act & Assert
            Assert.True(result.Errors[0].Message == "Cannot find any TimelineItem with corresponding id: 1");
        }

        [Fact]
        public async Task Handle_SuccessfulUpdate_ReturnsUpdatedTimelineItemDTO()
        {
            // Arrange
            var source = new TimelineItemDTO
            {
                Id = 1,
                Title = "Updated Title",
                Description = "Updated Description",
                Date = new DateTime(2004, 3, 13),
                DateViewPattern = DateViewPattern.DateMonthYear,
                HistoricalContexts = new List<HistoricalContextDTO>
                {
                    new HistoricalContextDTO { Id = 1, Title = "Context 1" },
                    new HistoricalContextDTO { Id = 5, Title = "Context 5" }
                }
            };
            
            var command = new UpdateTimelineItemCommand(source);

            var updatingTimelineItem = new TimelineItemEntity
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                Date = new DateTime(1999, 8, 7),
                DateViewPattern = DateViewPattern.SeasonYear,
                HistoricalContextTimelines = new List<HistoricalContextTimeline>()
            };

            MockRepositoryWrapper(updatingTimelineItem);

            MockRepositoryWrapper(new int[] { 1 });

            _mapperMock.Setup(m => m.Map<TimelineItemDTO>(It.IsAny<TimelineItemEntity>())).Returns(source);

            _handler = new UpdateTimelineItemHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryWrapperMock.Verify(r => r.TimelineRepository.Update(It.IsAny<TimelineItemEntity>()), Times.Once);
            _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
            Assert.True(result.IsSuccess);
            Assert.True(updatingTimelineItem.Title == source.Title &&
                updatingTimelineItem.Description == source.Description &&
                updatingTimelineItem.Date == source.Date &&
                updatingTimelineItem.DateViewPattern == source.DateViewPattern &&
                updatingTimelineItem.HistoricalContextTimelines.Count == 2 &&
                updatingTimelineItem.HistoricalContextTimelines.Any(h => h.HistoricalContextId == 1 && h.TimelineId == 1) &&
                updatingTimelineItem.HistoricalContextTimelines.Any(h => h.HistoricalContextId == 0 && h.TimelineId == 1));  
        }

        [Fact]
        public async Task Handle_ExceptionThrown_ThrowsInvalidOperationException()
        {
            // Arrange
            var source = new TimelineItemDTO
            {
                Id = 1,
                Title = "Updated Title",
                Description = "Updated Description",
                Date = new DateTime(2004, 3, 13),
                DateViewPattern = DateViewPattern.DateMonthYear,
                HistoricalContexts = new List<HistoricalContextDTO>
                {
                    new HistoricalContextDTO { Id = 1, Title = "Context 1" },
                    new HistoricalContextDTO { Id = 5, Title = "Context 5" }
                }
            };

            var command = new UpdateTimelineItemCommand(source);

            var updatingTimelineItem = new TimelineItemEntity
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                Date = new DateTime(1999, 8, 7),
                DateViewPattern = DateViewPattern.SeasonYear,
                HistoricalContextTimelines = new List<HistoricalContextTimeline>()
            };

            _mapperMock.Setup(m => m.Map<TimelineItemEntity>(command.sourceTimeLine)).Returns(updatingTimelineItem);

            MockRepositoryWrapper(updatingTimelineItem);

            MockRepositoryWrapper(new int[] { 1 });

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).Throws(new InvalidOperationException("Save failed"));

            _handler = new UpdateTimelineItemHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act&Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        private void MockRepositoryWrapper(int id = 1, bool returnNull = false)
        {
            _repositoryWrapperMock.Setup(r => r.TimelineRepository
                .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                IIncludableQueryable<TimelineItemEntity, object>>>()))
                .ReturnsAsync(returnNull ? null! : new TimelineItemEntity { Id = id });
        }

        private void MockRepositoryWrapper(TimelineItemEntity item)
        {
            _repositoryWrapperMock.Setup(r => r.TimelineRepository
                .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                IIncludableQueryable<TimelineItemEntity, object>>>()))
                .ReturnsAsync(item);
        }

        private void MockRepositoryWrapper(int[] ids)
        {
            List<HistoricalContext> list = new List<HistoricalContext>();
            foreach(int id in ids)
            {
                list.Add(new HistoricalContext { Id = id, Title = $"Context {id}" });
            }

            _repositoryWrapperMock.Setup(r => r.HistoricalContextRepository
                .GetAllAsync(
                It.IsAny<Expression<Func<HistoricalContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistoricalContext>,
                IIncludableQueryable<HistoricalContext, object>>>()))
                .ReturnsAsync(list);
        }
    }
}
