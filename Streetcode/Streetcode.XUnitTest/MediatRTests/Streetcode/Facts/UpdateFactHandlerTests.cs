using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Facts.Update;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Facts
{
    public class UpdateFactHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly UpdateFactHandler _handler;

        public UpdateFactHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new UpdateFactHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenFactIsUpdated()
        {
            var factDto = new FactUpdateCreateDto { Id = 1, FactContent = "Fact content" };
            var fact = new Fact { Id = 1, FactContent = "Content" };

            var command = new UpdateFactCommand(factDto);

            _mapperMock.Setup(m => m.Map<Fact>(factDto)).Returns(fact);
            _repositoryWrapperMock.Setup(r => r.FactRepository.Update(fact));
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenFactIsNull()
        {
            // Arrange
            _mapperMock.Setup(m => m.Map<Fact>(It.IsAny<FactDto>())).Returns<Fact>(null);
            var request = new UpdateFactCommand(new FactUpdateCreateDto());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnErrorMsg_WhenFactIsNull()
        {
            // Arrange
            _mapperMock.Setup(m => m.Map<Fact>(It.IsAny<FactDto>())).Returns<Fact>(null);
            var request = new UpdateFactCommand(new FactUpdateCreateDto());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal("Cannot convert null to Facts", result.Errors[0].Message);
        }
    }
}
