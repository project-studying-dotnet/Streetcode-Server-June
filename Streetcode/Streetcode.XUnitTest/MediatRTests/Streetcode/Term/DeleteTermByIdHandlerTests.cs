namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Term;

using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

using AutoMapper;
using Moq;
using Xunit;

using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.MediatR.Streetcode.Term.DeleteById;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

public class DeleteTermByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly DeleteTermByIdHandler _handler;

    public DeleteTermByIdHandlerTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new DeleteTermByIdHandler(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRelatedTermNotFound()
    {
        // Arrange
        var request = new DeleteTermByIdCommand(1);
        SetupRepositoryToReturnNull();
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(errorMsg, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRelatedTermDeleted()
    {
        // Arrange
        var relatedTerm = new Entity { Id = 1, Title = "Test" };
        var relatedTermDto = new TermDTO { Id = 1, Title = "Test" };
        SetupRepositoryToReturnEntity(relatedTerm);
        SetupRepositoryToSaveChanges(true);
        SetupMapper(relatedTerm, relatedTermDto);

        // Act
        var result = await _handler.Handle(new DeleteTermByIdCommand(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(relatedTermDto, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenSaveChangesFails()
    {
        // Arrange
        var relatedTerm = new Entity { Id = 1, Title = "Test" };
        var relatedTermDto = new TermDTO { Id = 1, Title = "Test" };
        var request = new DeleteTermByIdCommand(1);
        SetupRepositoryToReturnEntity(relatedTerm);
        SetupRepositoryToSaveChanges(false);
        SetupMapper(relatedTerm, relatedTermDto);
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToDeleteA, request);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(errorMsg, result.Errors.First().Message);
    }

    private void SetupRepositoryToReturnNull()
    {
        _mockRepository.Setup(x => x.TermRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Entity, bool>>>(), null))
            .ReturnsAsync((Entity?)null);
    }

    private void SetupRepositoryToReturnEntity(Entity relatedTerm)
    {
        _mockRepository.Setup(x => x.TermRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Entity, bool>>>(), null))
            .ReturnsAsync(relatedTerm);
    }

    private void SetupRepositoryToSaveChanges(bool isSuccess)
    {
        _mockRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(isSuccess ? 1 : 0);
    }

    private void SetupMapper(Entity relatedTerm, TermDTO relatedTermDto)
    {
        _mockMapper.Setup(mapper => mapper.Map<TermDTO>(relatedTerm)).Returns(relatedTermDto);
    }
}
