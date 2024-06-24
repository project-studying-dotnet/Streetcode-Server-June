using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace StreetcodeTest.XUnitTest.MediatRTests.StreetcodeTest.Facts
{
    public class DeleteFactHandlerTest
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DeleteFactHandler _handler;

        public DeleteFactHandlerTest()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _mockMapper = new Mock<IMapper>();
            _handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ReturnsFailResult_When_FactNotFound()
        {
            // Arrange
            var command = new DeleteFactCommand(1);
            _mockRepositoryWrapper.Setup(r => r.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                default))
                .ReturnsAsync((Fact)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task Handle_ReturnsOkResult_WhenFactExist()
        {
            // Arrange
            var command = new DeleteFactCommand(1);
            var fact = new Fact { Id = 1 };

            _mockRepositoryWrapper.Setup(r => r.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                default))
                .ReturnsAsync(fact);
            _mockRepositoryWrapper.Setup(r => r.FactRepository.Delete(fact));
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_DeleteFailed_ReturnsFailResult()
        {
            // Arrange
            var command = new DeleteFactCommand(1);
            var fact = new Fact { Id = 1 };

            _mockRepositoryWrapper.Setup(r => r.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                default))
                .ReturnsAsync(fact);
            _mockRepositoryWrapper.Setup(r => r.FactRepository.Delete(fact));
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }
    }
}
