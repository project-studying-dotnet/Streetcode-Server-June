namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Term;

using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.MediatR.Streetcode.Term.Delete;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

public class DeleteTermHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly DeleteTermHandler _handler;

    public DeleteTermHandlerTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new DeleteTermHandler(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenTermNotFound()
    {
        // Arrange
        var request = new DeleteTermCommand("test");
        MockRepositorySetup(true);
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

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

        // Act
        var result = await _handler.Handle(new DeleteTermCommand(word), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepository.Verify(x => x.TermRepository.Delete(It.IsAny<Entity>()), Times.Once);
        _mockMapper.Verify(x => x.Map<TermDTO>(It.IsAny<Entity>()), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        // Arrange
        var request = new DeleteTermCommand("test");
        MockRepositorySetup(false, "test");
        _mockRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);
        MockMapperSetup(false, "test");
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToDeleteA, request);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
        _mockLogger.Verify(x => x.LogError(It.IsAny<object>(), errorMsg), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMappingFails()
    {
        // Arrange
        var request = new DeleteTermCommand("test");
        MockRepositorySetup(false, "test");
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToDeleteA, request);
        _mockRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        MockMapperSetup(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
        _mockLogger.Verify(x => x.LogError(It.IsAny<object>(), errorMsg), Times.Once);
    }

    private void MockRepositorySetup(bool returnNull, string title = "")
    {
        _mockRepository.Setup(x => x.TermRepository
            .GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Entity, bool>>>(), null))
            .ReturnsAsync(returnNull ? null! : new Entity { Title = title });
    }

    private void MockMapperSetup(bool returnNull, string title = "")
    {
        _mockMapper.Setup(x => x.Map<TermDTO>(It.IsAny<Entity>()))
            .Returns(returnNull ? null! : new TermDTO { Title = title });
    }
}