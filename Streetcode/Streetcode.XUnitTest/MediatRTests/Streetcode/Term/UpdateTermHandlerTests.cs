namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Term;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;

using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.MediatR.Streetcode.Term.Update;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Text;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

public class UpdateTermHandlerTests
{
    private readonly Mock<IRepositoryWrapper> mockRepo;
    private readonly Mock<IMapper> mockMapper;
    private readonly Mock<ILoggerService> mockLogger;
    private readonly UpdateTermHandler handler;

    public UpdateTermHandlerTests()
    {
        mockRepo = new Mock<IRepositoryWrapper>();
        mockMapper = new Mock<IMapper>();
        mockLogger = new Mock<ILoggerService>();
        handler = new UpdateTermHandler(mockMapper.Object, mockRepo.Object, mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUpdatedRelatedTermDto_WhenSuccess()
    {
        // Arrange
        var request = ArrangeMocksForSuccess();
        var expectedResult = GetUpdatedTermDto();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResult.Id, result.Value.Id);
        Assert.Equal(expectedResult.Title, result.Value.Title);
        Assert.Equal(expectedResult.Description, result.Value.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailResult_WhenRelatedTermNotFound()
    {
        // Arrange
        var request = ArrangeMocksForNotFound();
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMsg, result.Errors.First().Message);
        mockLogger.Verify(logger => logger.LogError(request, errorMsg), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailResult_WhenTermAlreadyExists()
    {
        // Arrange
        var request = ArrangeMocksForTermAlreadyExists();
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.TermAlreadyExist, request);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMsg, result.Errors.First().Message);
        mockLogger.Verify(logger => logger.LogError(request, errorMsg), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailResult_WhenSavingFails()
    {
        // Arrange
        var request = ArrangeMocksForSavingFails();
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToUpdate, request);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMsg, result.Errors.First().Message);
        mockLogger.Verify(logger => logger.LogError(request, errorMsg), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailResult_WhenMappingFails()
    {
        // Arrange
        var request = ArrangeMocksForMappingFails();
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMsg, result.Errors.First().Message);
        mockLogger.Verify(logger => logger.LogError(request, errorMsg), Times.Once);
    }

    private UpdateTermCommand ArrangeMocksForSuccess()
    {
        var relatedTermDto = GetUpdatedTermDto();
        var relatedTermEntity = GetTermEntity();
        var updatedTermEntity = GetUpdatedTermEntity();

        mockRepo.Setup(repo => repo.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(relatedTermEntity);

        mockRepo.Setup(repo => repo.TermRepository.GetAllAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity>());

        mockMapper.Setup(mapper => mapper.Map<TermDTO>(updatedTermEntity.Entity)).Returns(relatedTermDto);
        mockMapper.Setup(mapper => mapper.Map<Entity>(relatedTermDto)).Returns(updatedTermEntity.Entity);
        mockRepo.Setup(repo => repo.TermRepository.Update(It.IsAny<Entity>())).Returns(updatedTermEntity);
        mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        return new UpdateTermCommand(relatedTermDto);
    }

    private UpdateTermCommand ArrangeMocksForNotFound()
    {
        var termDto = GetUpdatedTermDto();

        mockRepo.Setup(repo => repo.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(null as Entity);

        return new UpdateTermCommand(termDto);
    }

    private UpdateTermCommand ArrangeMocksForTermAlreadyExists()
    {
        var termDto = GetUpdatedTermDto();
        var termEntity = GetTermEntity();

        mockRepo.Setup(repo => repo.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(termEntity);

        mockRepo.Setup(repo => repo.TermRepository.GetAllAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity> { termEntity });

        return new UpdateTermCommand(termDto);
    }

    private UpdateTermCommand ArrangeMocksForSavingFails()
    {
        var relatedTermDto = GetUpdatedTermDto();
        var relatedTermEntity = GetTermEntity();
        var updatedTermEntity = GetUpdatedTermEntity();

        mockRepo.Setup(repo => repo.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(relatedTermEntity);

        mockRepo.Setup(repo => repo.TermRepository.GetAllAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity>());

        mockMapper.Setup(mapper => mapper.Map<Entity>(relatedTermDto)).Returns(updatedTermEntity.Entity);
        mockRepo.Setup(repo => repo.TermRepository.Update(It.IsAny<Entity>())).Returns(updatedTermEntity);
        mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

        return new UpdateTermCommand(relatedTermDto);
    }

    private UpdateTermCommand ArrangeMocksForMappingFails()
    {
        var termDto = GetUpdatedTermDto();
        var termEntity = GetTermEntity();
        var updatedTermEntity = GetUpdatedTermEntity();

        mockRepo.Setup(repo => repo.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(termEntity);

        mockRepo.Setup(repo => repo.TermRepository.GetAllAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity>());

        mockMapper.Setup(mapper => mapper.Map<Entity>(termDto)).Returns(updatedTermEntity.Entity);
        mockRepo.Setup(repo => repo.TermRepository.Update(It.IsAny<Entity>())).Returns(updatedTermEntity);
        mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        mockMapper.Setup(mapper => mapper.Map<TermDTO>(updatedTermEntity.Entity)).Returns((TermDTO)null);

        return new UpdateTermCommand(termDto);
    }

    private Entity GetTermEntity() => new Entity { Id = 1, Title = "Original Term", Description = "Original Description" };

    private EntityEntry<Entity> GetUpdatedTermEntity()
    {
        var contextOptions = new DbContextOptionsBuilder<MockDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new MockDbContext(contextOptions);
        var updatedEntity = new Entity { Id = 1, Title = "Updated Term", Description = "Updated Description" };
        context.Add(updatedEntity);
        context.SaveChanges();

        return context.Entry(updatedEntity);
    }

    private TermDTO GetUpdatedTermDto() => new TermDTO { Id = 1, Title = "Updated Term", Description = "Updated Description" };
}
