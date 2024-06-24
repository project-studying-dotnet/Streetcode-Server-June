namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Text;

using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Text.Update;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;
using System.Linq.Expressions;
using Streetcode.DAL.Entities.Sources;
using Microsoft.EntityFrameworkCore;

public class UpdateTextHandlerTests
{
    private readonly Mock<IRepositoryWrapper> mockRepo;
    private readonly Mock<IMapper> mockMapper;
    private readonly Mock<ILoggerService> mockLogger;
    private readonly UpdateTextHandler handler;

    public UpdateTextHandlerTests()
    {
        mockRepo = new Mock<IRepositoryWrapper>();
        mockMapper = new Mock<IMapper>();
        mockLogger = new Mock<ILoggerService>();
        handler = new UpdateTextHandler(mockRepo.Object, mockLogger.Object, mockMapper.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUpdatedTextDto_WhenSuccess()
    {
        // Arrange
        var request = ArrangeMocksForSuccess();
        var expectedResult = GetTextDto();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResult.Id, result.Value.Id);
        Assert.Equal(expectedResult.Title, result.Value.Title);
        Assert.Equal(expectedResult.TextContent, result.Value.TextContent);
        Assert.Equal(expectedResult.StreetcodeId, result.Value.StreetcodeId);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailResult_WhenTextNotFound()
    {
        // Arrange
        var request = ArrangeMocksForNotFound();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal($"Entity with id: {request.Id} not found", result.Errors.First().Message);
        mockLogger.Verify(logger => logger.LogError(request, $"Entity with id: {request.Id} not found"), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailResult_WhenSavingFails()
    {
        // Arrange
        var request = ArrangeMocksForSavingFails();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Fail to update the entity", result.Errors.First().Message);
        mockLogger.Verify(logger => logger.LogError(request, "Fail to update the entity"), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailResult_WhenMappingToDtoFails()
    {
        // Arrange
        var request = ArrangeMocksForMappingToDtoFails();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Fail to map the entity", result.Errors.First().Message);
        mockLogger.Verify(logger => logger.LogError(request, "Fail to map the entity"), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenRepositoryThrowsException()
    {
        // Arrange
        var request = ArrangeMocksForException();

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.Equal("Database error", exception.Message);
    }

    private UpdateTextCommand ArrangeMocksForSuccess()
    {
        var textUpdateDto = GetTextUpdateDto();
        var textEntity = GetTextEntity();
        var updatedTextEntity = GetUpdatedTextEntity();
        var textDto = GetTextDto();

        mockRepo.Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(textEntity);

        mockMapper.Setup(mapper => mapper.Map<TextDTO>(updatedTextEntity.Entity)).Returns(textDto);
        mockMapper.Setup(mapper => mapper.Map<Entity>(textUpdateDto)).Returns(updatedTextEntity.Entity);
        mockRepo.Setup(repo => repo.TextRepository.Update(It.IsAny<Entity>())).Returns(updatedTextEntity);

        mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);


        return new UpdateTextCommand(1, textUpdateDto);
    }

    private UpdateTextCommand ArrangeMocksForNotFound()
    {
        var textUpdateDto = GetTextUpdateDto();

        mockRepo.Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(null as Entity);

        return new UpdateTextCommand(1, textUpdateDto);
    }

    private UpdateTextCommand ArrangeMocksForSavingFails()
    {
        var textUpdateDto = GetTextUpdateDto();
        var textEntity = GetTextEntity();
        var updatedTextEntity = GetUpdatedTextEntity();

        mockRepo.Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(textEntity);

        mockMapper.Setup(mapper => mapper.Map<Entity>(textUpdateDto)).Returns(updatedTextEntity.Entity);
        mockRepo.Setup(repo => repo.TextRepository.Update(It.IsAny<Entity>()));
        mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

        return new UpdateTextCommand(1, textUpdateDto);
    }

    private UpdateTextCommand ArrangeMocksForMappingToDtoFails()
    {
        var textUpdateDto = GetTextUpdateDto();
        var textEntity = GetTextEntity();
        var updatedTextEntity = GetUpdatedTextEntity();

        mockRepo.Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(textEntity);

        mockMapper.Setup(mapper => mapper.Map<Entity>(textUpdateDto)).Returns(updatedTextEntity.Entity);
        mockRepo.Setup(repo => repo.TextRepository.Update(It.IsAny<Entity>()));
        mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        mockMapper.Setup(mapper => mapper.Map<TextDTO>(updatedTextEntity)).Returns((null as TextDTO)!);

        return new UpdateTextCommand(1, textUpdateDto);
    }

    private UpdateTextCommand ArrangeMocksForException()
    {
        var textUpdateDto = GetTextUpdateDto();
        var textEntity = GetTextEntity();
        var updatedTextEntity = GetUpdatedTextEntity();

        mockRepo.Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(textEntity);

        mockMapper.Setup(mapper => mapper.Map<Entity>(textUpdateDto)).Returns(updatedTextEntity.Entity);
        mockRepo.Setup(repo => repo.TextRepository.Update(It.IsAny<Entity>())).Returns(It.IsAny<EntityEntry<Entity>>());
        mockRepo.Setup(repo => repo.SaveChangesAsync()).ThrowsAsync(new Exception("Database error"));

        return new UpdateTextCommand(1, textUpdateDto);
    }

    private TextCreateDTO GetTextUpdateDto() => new TextCreateDTO { Title = "Updated Title", TextContent = "Updated Content", StreetcodeId = 2 };

    private Entity GetTextEntity() => new Entity { Id = 1, Title = "Original Title", TextContent = "Original Content", StreetcodeId = 1 };

    private EntityEntry<Entity> GetUpdatedTextEntity()
    {
        var contextOptions = new DbContextOptionsBuilder<MockDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new MockDbContext(contextOptions);
        var updatedEntity = new Entity { Id = 1, Title = "Updated Title", TextContent = "Updated Content", StreetcodeId = 2 };
        context.Add(updatedEntity);
        context.SaveChanges();

        return context.Entry(updatedEntity);
    }

    private TextDTO GetTextDto() => new TextDTO { Id = 1, Title = "Updated Title", TextContent = "Updated Content", StreetcodeId = 2 };
}
