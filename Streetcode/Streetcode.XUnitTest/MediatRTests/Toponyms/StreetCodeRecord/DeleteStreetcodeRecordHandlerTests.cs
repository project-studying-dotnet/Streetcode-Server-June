using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Toponyms.StreetCodeRecord.Delete;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Toponyms.StreetCodeRecord
{
    public class DeleteStreetcodeRecordHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _wrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private StreetcodeToponym _record = new StreetcodeToponym { StreetcodeId = 1, ToponymId = 2 };
        private StreetcodeToponym _nullRecord;
        public DeleteStreetcodeRecordHandlerTests()
        {
            _wrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenRecordIsNotNull()
        {
            // Arrange
            var query = new DeleteStreetcodeRecordCommand(1, 2);
            var handler = new DeleteStreetcodeRecordHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
            _wrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            _wrapperMock.Setup(obj => obj.StreetcodeToponymRepository.GetFirstOrDefaultAsync(
                                                                     It.IsAny<Expression<Func<StreetcodeToponym, bool>>>(), default))
                                                                     .ReturnsAsync(_record);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenRecordIsNull()
        {
            // Arrange
            var query = new DeleteStreetcodeRecordCommand(1, 2);
            var handler = new DeleteStreetcodeRecordHandler(_wrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
            _wrapperMock.Setup(obj => obj.StreetcodeToponymRepository.GetFirstOrDefaultAsync(
                                                                     It.IsAny<Expression<Func<StreetcodeToponym, bool>>>(), default))
                                                                     .ReturnsAsync(_nullRecord);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}
