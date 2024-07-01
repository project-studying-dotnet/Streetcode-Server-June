namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Term;

using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

using Streetcode.BLL.Resources;
using BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Term.Create;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.DAL.Repositories.Interfaces.Base;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

public class CreateTermHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;

    public CreateTermHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
        _mockMapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenRelatedTermIsNull()
    {
        // Arrange
        MockRepositorySetupNullOrEmptyArrOffIds();
        var command = new CreateTermCommand(null!);
        var handler = new CreateTermHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, command);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenRelatedTermAlreadyExists()
    {
        // Arrange
        var request = GetValidCreateRelatedTermRequest();
        SetupMockForExistingTerm(request);
        var handler = CreateHandler();
        var command = new CreateTermCommand(request);
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.TermAlreadyExist, request);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenSaveChangesFails()
    {
        // Arrange
        var request = GetValidCreateRelatedTermRequest();
        SetupMockForSaveChangesFail(request);
        var handler = CreateHandler();
        var command = new CreateTermCommand(request);
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.CanNotCreate, request);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenMappingFails()
    {
        // Arrange
        var request = GetValidCreateRelatedTermRequest();
        SetupMockForMappingFail(request);
        var handler = CreateHandler();
        var command = new CreateTermCommand(request);
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors.FirstOrDefault()?.Message);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenOperationIsSuccessful()
    {
        // Arrange
        var request = GetValidCreateRelatedTermRequest();
        SetupMockForSuccess(request);
        var handler = CreateHandler();
        var command = new CreateTermCommand(request);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Value.Should().BeEquivalentTo(request);
    }

    private CreateTermHandler CreateHandler()
    {
        return new CreateTermHandler(
            repository: _mockRepositoryWrapper.Object,
            mapper: _mockMapper.Object,
            logger: _mockLogger.Object);
    }

    private void MockRepositorySetupNullOrEmptyArrOffIds()
    {
        _mockRepositoryWrapper.Setup(x => x.TermRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))!
            .ReturnsAsync((IEnumerable<Entity>?)null);
    }

    private void SetupMockForExistingTerm(TermCreateDTO request)
    {
        _mockMapper.Setup(m => m.Map<Entity>(It.IsAny<TermCreateDTO>())).Returns(new Entity());

        _mockRepositoryWrapper.Setup(x => x.TermRepository
                .GetAllAsync(
                    It.Is<Expression<Func<Entity, bool>>>(predicate =>
                        predicate.Compile().Invoke(new Entity { Title = request.Title, Description = request.Description })),
                    It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity>
            {
                    new Entity()
                    {
                        Id = 1,
                        Title = request.Title,
                        Description = request.Description
                    },
            });
    }

    private void SetupMockForSaveChangesFail(TermCreateDTO request)
    {
        _mockMapper.Setup(m => m.Map<Entity>(It.IsAny<TermCreateDTO>())).Returns(new Entity());

        _mockRepositoryWrapper.Setup(r => r.TermRepository
                .GetAllAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity>());

        _mockRepositoryWrapper.Setup(r => r.TermRepository.Create(It.IsAny<Entity>()));

        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);
    }

    private void SetupMockForMappingFail(TermCreateDTO request)
    {
        _mockMapper.Setup(m => m.Map<Entity>(It.IsAny<TermCreateDTO>())).Returns(new Entity());

        _mockRepositoryWrapper.Setup(r => r.TermRepository
                .GetAllAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity>());

        _mockRepositoryWrapper.Setup(r => r.TermRepository.Create(It.IsAny<Entity>()));

        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        _mockMapper.Setup(m => m.Map<TermDTO>(It.IsAny<Entity>())).Returns((TermDTO)null!);
    }

    private void SetupMockForSuccess(TermCreateDTO request)
    {
        var relatedTermEntity = new Entity
        {
            Id = 1,
            Title = request.Title,
            Description = request.Description,
        };

        var relatedTermDto = new TermDTO
        {
            Id = 1,
            Title = request.Title,
            Description = request.Description,
        };

        _mockMapper.Setup(m => m.Map<Entity>(It.IsAny<TermCreateDTO>())).Returns(relatedTermEntity);

        _mockRepositoryWrapper.Setup(r => r.TermRepository
                .GetAllAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(new List<Entity>());

        _mockRepositoryWrapper.Setup(r => r.TermRepository.Create(It.IsAny<Entity>()));

        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        _mockMapper.Setup(m => m.Map<TermDTO>(It.IsAny<Entity>())).Returns(relatedTermDto);
    }

    private TermCreateDTO GetValidCreateRelatedTermRequest()
    {
        return new TermCreateDTO
        {
            Title = "Title Title",
            Description = "Description Description",
        };
    }
}
