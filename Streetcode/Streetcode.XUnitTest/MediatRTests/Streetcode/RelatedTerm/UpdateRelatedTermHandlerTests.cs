namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.RelatedTerm;

using System.Linq.Expressions;

using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;

using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Text;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

public class UpdateRelatedTermHandlerTests
{
    private readonly Mock<IRepositoryWrapper> mockRepo;
    private readonly Mock<IMapper> mockMapper;
    private readonly Mock<ILoggerService> mockLogger;
    private readonly UpdateRelatedTermHandler handler;

    public UpdateRelatedTermHandlerTests()
    {
        mockRepo = new Mock<IRepositoryWrapper>();
        mockMapper = new Mock<IMapper>();
        mockLogger = new Mock<ILoggerService>();
        handler = new UpdateRelatedTermHandler(mockMapper.Object, mockRepo.Object, mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUpdatedRelatedTermDto_WhenSuccess()
    {
        // Arrange
        var request = ArrangeMocksForSuccess();
        var expectedResult = GetUpdatedRelatedTermDto();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResult.Id, result.Value.Id);
        Assert.Equal(expectedResult.Word, result.Value.Word);
        Assert.Equal(expectedResult.TermId, result.Value.TermId);
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

    private UpdateRelatedTermCommand ArrangeMocksForSuccess()
    {
        var relatedTermDto = GetUpdatedRelatedTermDto();
        var relatedTermEntity = GetRelatedTermEntity();
        var updatedRelatedTermEntity = GetUpdatedRelatedTermEntity();

        mockRepo.Setup(repo => repo.RelatedTermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(relatedTermEntity);

        mockRepo.Setup(repo => repo.RelatedTermRepository.GetAllAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity>());

        mockMapper.Setup(mapper => mapper.Map<RelatedTermDTO>(updatedRelatedTermEntity.Entity)).Returns(relatedTermDto);
        mockMapper.Setup(mapper => mapper.Map<Entity>(relatedTermDto)).Returns(updatedRelatedTermEntity.Entity);
        mockRepo.Setup(repo => repo.RelatedTermRepository.Update(It.IsAny<Entity>())).Returns(updatedRelatedTermEntity);
        mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        return new UpdateRelatedTermCommand(relatedTermDto);
    }

    private UpdateRelatedTermCommand ArrangeMocksForNotFound()
    {
        var relatedTermDto = GetUpdatedRelatedTermDto();

        mockRepo.Setup(repo => repo.RelatedTermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(null as Entity);

        return new UpdateRelatedTermCommand(relatedTermDto);
    }

    private UpdateRelatedTermCommand ArrangeMocksForTermAlreadyExists()
    {
        var relatedTermDto = GetUpdatedRelatedTermDto();
        var relatedTermEntity = GetRelatedTermEntity();

        mockRepo.Setup(repo => repo.RelatedTermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(relatedTermEntity);

        mockRepo.Setup(repo => repo.RelatedTermRepository.GetAllAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity> { relatedTermEntity });

        return new UpdateRelatedTermCommand(relatedTermDto);
    }

    private UpdateRelatedTermCommand ArrangeMocksForSavingFails()
    {
        var relatedTermDto = GetUpdatedRelatedTermDto();
        var relatedTermEntity = GetRelatedTermEntity();
        var updatedRelatedTermEntity = GetUpdatedRelatedTermEntity();

        mockRepo.Setup(repo => repo.RelatedTermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(relatedTermEntity);

        mockRepo.Setup(repo => repo.RelatedTermRepository.GetAllAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity>());

        mockMapper.Setup(mapper => mapper.Map<Entity>(relatedTermDto)).Returns(updatedRelatedTermEntity.Entity);
        mockRepo.Setup(repo => repo.RelatedTermRepository.Update(It.IsAny<Entity>())).Returns(updatedRelatedTermEntity);
        mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

        return new UpdateRelatedTermCommand(relatedTermDto);
    }

    private UpdateRelatedTermCommand ArrangeMocksForMappingFails()
    {
        var relatedTermDto = GetUpdatedRelatedTermDto();
        var relatedTermEntity = GetRelatedTermEntity();
        var updatedRelatedTermEntity = GetUpdatedRelatedTermEntity();

        mockRepo.Setup(repo => repo.RelatedTermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(relatedTermEntity);

        mockRepo.Setup(repo => repo.RelatedTermRepository.GetAllAsync(
                It.IsAny<Expression<Func<Entity, bool>>>(),
                It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity>());

        mockMapper.Setup(mapper => mapper.Map<Entity>(relatedTermDto)).Returns(updatedRelatedTermEntity.Entity);
        mockRepo.Setup(repo => repo.RelatedTermRepository.Update(It.IsAny<Entity>())).Returns(updatedRelatedTermEntity);
        mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        mockMapper.Setup(mapper => mapper.Map<RelatedTermDTO>(updatedRelatedTermEntity.Entity)).Returns((RelatedTermDTO)null);

        return new UpdateRelatedTermCommand(relatedTermDto);
    }

    private Entity GetRelatedTermEntity() => new Entity { Id = 1, Word = "Original Term", TermId = 1 };

    private EntityEntry<Entity> GetUpdatedRelatedTermEntity()
    {
        var contextOptions = new DbContextOptionsBuilder<MockDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new MockDbContext(contextOptions);
        var updatedEntity = new Entity { Id = 1, Word = "Updated Term", TermId = 2 };
        context.Add(updatedEntity);
        context.SaveChanges();

        return context.Entry(updatedEntity);
    }

    private RelatedTermDTO GetUpdatedRelatedTermDto() => new RelatedTermDTO { Id = 1, Word = "Updated Term", TermId = 2 };
}
