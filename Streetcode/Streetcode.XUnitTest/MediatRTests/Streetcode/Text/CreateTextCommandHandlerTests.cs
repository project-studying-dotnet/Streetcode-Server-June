using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentResults;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Text.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Text
{
    public class CreateTextCommandHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> mockRepo;
        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<ILoggerService> mockLogger;
        private readonly CreateTextCommandHandler handler;

        public CreateTextCommandHandlerTests()
        {
            mockRepo = new Mock<IRepositoryWrapper>();
            mockMapper = new Mock<IMapper>();
            mockLogger = new Mock<ILoggerService>();
            handler = new CreateTextCommandHandler(mockRepo.Object, mockMapper.Object, mockLogger.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnsCreatedTextDto_WhenSuccess()
        {
            // Arrange
            var textCreateDTO = new TextCreateDTO { Title = "Test Title", TextContent = "Test Content", StreetcodeId = 1 };
            var textEntity = new Entity { Id = 1, Title = "Test Title", TextContent = "Test Content", StreetcodeId = 1 };
            var textDTO = new TextDTO { Id = 1, Title = "Test Title", TextContent = "Test Content", StreetcodeId = 1 };

            mockMapper.Setup(mapper => mapper.Map<Entity>(textCreateDTO)).Returns(textEntity);
            mockRepo.Setup(repo => repo.TextRepository.CreateAsync(textEntity)).ReturnsAsync(textEntity);
            mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
            mockMapper.Setup(mapper => mapper.Map<TextDTO>(textEntity)).Returns(textDTO);

            var request = new CreateTextCommand(textCreateDTO);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(textDTO, result.Value);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailResult_WhenMappingFails()
        {
            // Arrange
            var textCreateDTO = new TextCreateDTO { Title = "Test Title", TextContent = "Test Content", StreetcodeId = 1 };

            mockMapper.Setup(mapper => mapper.Map<Entity>(textCreateDTO)).Returns((Entity)null);

            var request = new CreateTextCommand(textCreateDTO);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Cannot create new Texts entity!", result.Errors.First().Message);
            mockLogger.Verify(logger => logger.LogError(request, "Cannot create new Texts entity!"), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailResult_WhenSavingFails()
        {
            // Arrange
            var textCreateDTO = new TextCreateDTO { Title = "Test Title", TextContent = "Test Content", StreetcodeId = 1 };
            var textEntity = new Entity { Id = 1, Title = "Test Title", TextContent = "Test Content", StreetcodeId = 1 };

            mockMapper.Setup(mapper => mapper.Map<Entity>(textCreateDTO)).Returns(textEntity);
            mockRepo.Setup(repo => repo.TextRepository.CreateAsync(textEntity)).ReturnsAsync(textEntity);
            mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

            var request = new CreateTextCommand(textCreateDTO);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Cannot save changes in the database after Texts creation!", result.Errors.First().Message);
            mockLogger.Verify(logger => logger.LogError(request, "Cannot save changes in the database after Texts creation!"), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailResult_WhenMappingToDtoFails()
        {
            // Arrange
            var textCreateDTO = new TextCreateDTO { Title = "Test Title", TextContent = "Test Content", StreetcodeId = 1 };
            var textEntity = new Entity { Id = 1, Title = "Test Title", TextContent = "Test Content", StreetcodeId = 1 };

            mockMapper.Setup(mapper => mapper.Map<Entity>(textCreateDTO)).Returns(textEntity);
            mockRepo.Setup(repo => repo.TextRepository.CreateAsync(textEntity)).ReturnsAsync(textEntity);
            mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
            mockMapper.Setup(mapper => mapper.Map<TextDTO>(textEntity)).Returns((TextDTO)null);

            var request = new CreateTextCommand(textCreateDTO);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Cannot map entity!", result.Errors.First().Message);
            mockLogger.Verify(logger => logger.LogError(request, "Cannot map entity!"), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var textCreateDTO = new TextCreateDTO { Title = "Test Title", TextContent = "Test Content", StreetcodeId = 1 };
            var textEntity = new Entity { Id = 1, Title = "Test Title", TextContent = "Test Content", StreetcodeId = 1 };

            mockMapper.Setup(mapper => mapper.Map<Entity>(textCreateDTO)).Returns(textEntity);
            mockRepo.Setup(repo => repo.TextRepository.CreateAsync(textEntity)).ReturnsAsync(textEntity);
            mockRepo.Setup(repo => repo.SaveChangesAsync()).ThrowsAsync(new Exception("Database error"));

            var request = new CreateTextCommand(textCreateDTO);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(request, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }
    }
}
