namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.RelatedTerm;

using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

public class DeleteRelatedTermHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public DeleteRelatedTermHandlerTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenTermNotFound()
    {
        // Arrange
        var request = new DeleteRelatedTermCommand("test");
        MockRepositorySetup(true);
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
        var handler = new DeleteRelatedTermHandler(
            _mockRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
        _mockLogger.Verify(x => x.LogError(It.IsAny<object>(), errorMsg), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTermDeletedSuccessfully()
    {
        // Arrange
        string word = "test";
        MockRepositorySetup(false, word);
        _mockRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        MockMapperSetup(false, word);

        var handler = new DeleteRelatedTermHandler(
            _mockRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new DeleteRelatedTermCommand(word), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepository.Verify(x => x.RelatedTermRepository.Delete(It.IsAny<Entity>()), Times.Once);
        _mockMapper.Verify(x => x.Map<RelatedTermDTO>(It.IsAny<Entity>()), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        // Arrange
        var request = new DeleteRelatedTermCommand("test");
        MockRepositorySetup(false, "test");
        _mockRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);
        MockMapperSetup(false, "test");
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToDeleteA, request);
        var handler = new DeleteRelatedTermHandler(
            _mockRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
        _mockLogger.Verify(x => x.LogError(It.IsAny<object>(), errorMsg), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMappingFails()
    {
        // Arrange
        var request = new DeleteRelatedTermCommand("test");
        MockRepositorySetup(false, "test");
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToDeleteA, request);
        _mockRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        MockMapperSetup(true);

        var handler = new DeleteRelatedTermHandler(
        _mockRepository.Object,
        _mockMapper.Object,
        _mockLogger.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
        _mockLogger.Verify(x => x.LogError(It.IsAny<object>(), errorMsg), Times.Once);
    }

    private void MockRepositorySetup(bool returnNull, string word = "")
    {
        _mockRepository.Setup(x => x.RelatedTermRepository
            .GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Entity, bool>>>(), null))
            .ReturnsAsync(returnNull ? null! : new Entity { Word = word });
    }

    private void MockMapperSetup(bool returnNull, string word = "")
    {
        _mockMapper.Setup(x => x.Map<RelatedTermDTO>(It.IsAny<Entity>()))
            .Returns(returnNull ? null! : new RelatedTermDTO { Word = word });
    }
}