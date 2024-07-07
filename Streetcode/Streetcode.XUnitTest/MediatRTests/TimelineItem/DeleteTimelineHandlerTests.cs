using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using TimelineEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem
{
    public class DeleteTimelineItemHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;

        public DeleteTimelineItemHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenTimelineItemFound()
        {
            // Arrange
            MockRepositoryWrapperSetupWithExistingTimelineItemId(1);

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            // Act
            var result = await handler.Handle(new DeleteTimelineItemCommand(1), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailResult_WhenTimelineItemNotFound()
        {
            // Arrange
            MockRepositoryWrapperSetupWithNotExistingTimelineItemyId();

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            // Act
            var result = await handler.Handle(new DeleteTimelineItemCommand(1), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task Handle_Should_LogCorrectErrorMessage_WhenTimelineItemIsNotFound()
        {
            // Arrange
            MockRepositoryWrapperSetupWithNotExistingTimelineItemyId();

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            var expectedErrorMessage = "Cannot find any TimelineItem";

            // Act
            var result = await handler.Handle(new DeleteTimelineItemCommand(1), CancellationToken.None);
            var actualErrorMessage = result.Errors[0].Message;

            // Assert
            Assert.Equal(expectedErrorMessage, actualErrorMessage);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailResult_WhenSaveChangesAsyncNotSuccessful()
        {
            // Arrange
            MockRepositoryWrapperSetupWithExistingTimelineItemId(1);
            _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            // Act
            var result = await handler.Handle(new DeleteTimelineItemCommand(1), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task Handle_Should_LogCorrectErrorMessage_WhenSaveChangesAsyncNotSuccessful()
        {
            // Arrange
            MockRepositoryWrapperSetupWithExistingTimelineItemId(1);
            _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            var expectedErrorMessage = "Failed to delete a TimelineItem";

            // Act
            var result = await handler.Handle(new DeleteTimelineItemCommand(1), CancellationToken.None);

            var actualErrorMessage = result.Errors[0].Message;

            // Assert
            Assert.Equal(expectedErrorMessage, actualErrorMessage);
        }

        private static TimelineEntity GetTimelineItem(int id)
        {
            return new TimelineEntity
            {
                Id = id,
            };
        }

        private void MockRepositoryWrapperSetupWithExistingTimelineItemId(int id)
        {
            _mockRepositoryWrapper.Setup(x => x.TimelineRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TimelineEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineEntity>,
                    IIncludableQueryable<TimelineEntity, object>>>()))
                .ReturnsAsync(GetTimelineItem(id));

            _mockRepositoryWrapper.Setup(x => x.TimelineRepository
                .Delete(GetTimelineItem(id)));

            _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        }

        private void MockRepositoryWrapperSetupWithNotExistingTimelineItemyId()
        {
            _mockRepositoryWrapper.Setup(x => x.TimelineRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TimelineEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineEntity>,
                    IIncludableQueryable<TimelineEntity, object>>>()))
                .ReturnsAsync((TimelineEntity)null!);
        }
    }
}