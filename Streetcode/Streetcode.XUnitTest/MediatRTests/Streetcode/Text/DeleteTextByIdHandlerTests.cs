namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Text;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Text.DeleteById;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

public class DeleteTextByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> mockRepo;
    private readonly Mock<ILoggerService> mockLogger;

    public DeleteTextByIdHandlerTests()
    {
        mockRepo = new Mock<IRepositoryWrapper>();
        mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_Should_ReturnOkResult_WhenTextIsDeletedSuccessfully()
    {
        // Arrange
        var handler = CreateHandler(true, true);

        // Act
        var result = await handler.Handle(new DeleteTextByIdCommand(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailResult_WhenTextNotFound()
    {
        // Arrange
        var handler = CreateHandler(false, false);
        var request = new DeleteTextByIdCommand(1);
        var expectedErrorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedErrorMsg, result.Errors.First().Message);
        mockLogger.Verify(logger => logger.LogError(request, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailResult_WhenSaveChangesFails()
    {
        // Arrange
        var handler = CreateHandler(true, false);
        var request = new DeleteTextByIdCommand(1);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        mockLogger.Verify(logger => logger.LogError(request, It.IsAny<string>()), Times.Once);
    }

    private DeleteTextByIdHandler CreateHandler(bool textExists, bool saveChangesSuccess)
    {
        MockRepository(textExists, saveChangesSuccess);

        return new DeleteTextByIdHandler(mockRepo.Object, mockLogger.Object);
    }

    private void MockRepository(bool textExists, bool saveChangesSuccess)
    {
        if (textExists)
        {
            mockRepo.Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Text, bool>>>(),
                    It.IsAny<Func<IQueryable<Text>, IIncludableQueryable<Text, object>>>()))
                .ReturnsAsync(new Text { Id = 1 }); 
            
            mockRepo.Setup(repo => repo.TextRepository.Delete(It.IsAny<Text>()));
        }
        else
        {
            mockRepo.Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Text, bool>>>(),
                    It.IsAny<Func<IQueryable<Text>, IIncludableQueryable<Text, object>>>()))
                .ReturnsAsync(null as Text);
        }

        mockRepo.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(saveChangesSuccess ? 1 : 0);
    }
}
