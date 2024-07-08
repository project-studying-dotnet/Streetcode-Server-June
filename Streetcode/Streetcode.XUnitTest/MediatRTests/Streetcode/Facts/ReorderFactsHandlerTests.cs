using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Facts;

public class ReorderFactsCommandHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ILoggerService> _loggerServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ReorderFactsHandler _handler;

    public ReorderFactsCommandHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerServiceMock = new Mock<ILoggerService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new ReorderFactsHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _loggerServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenListWthithNewPositionIsEmpty()
    {
        // Arrange
        var request = new ReorderFactsCommand(new List<FactUpdatePositionDto>(), 1);
        IError msgError = new Error("Updated list of position cannot be empty or null");

        SetupMocks(0, Enumerable.Empty<Fact>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Errors?.Exists(e => e.Message == msgError.Message));
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenNoFactsFound()
    {
        // Arrange
        var request = GetTestReorderFactsCommand();
        IError msgError = new Error("Cannot find any Fact by a streetcode id: 1");
        
        SetupMocks(1, Enumerable.Empty<Fact>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal("Cannot find any Fact by a streetcode id: 1", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenSaveChangesFails()
    {
        // Arrange
        var request = GetTestReorderFactsCommand();
        IError msgError = new Error("Failed to save changes to the database.");
        var facts = GetTestFacts();

        SetupMocks(0, facts, facts, Enumerable.Empty<FactDto>());
         
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Errors?.Exists(e => e.Message == msgError.Message));
    }

    [Fact]
    public async Task Handle_ShouldUpdatePositionsSuccessfully()
    {
        // Arrange
        var facts = GetTestFacts();
        var factsNew = GetExpectedFactsNew();
        var request = GetTestReorderFactsCommand();

        SetupMocks(1, facts, facts, factsNew);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(result.Value, factsNew);
    }

    private void SetupMocks(int saveChangesResult, IEnumerable<Fact>? firstOrDefaultFacts = default, IEnumerable<Fact>? facts = default, IEnumerable<FactDto>? returnDto = default)
    {
        _repositoryWrapperMock.Setup(repo => repo.FactRepository.GetAllAsync(
            It.IsAny<Expression<Func<Fact, bool>>?>(),
            It.IsAny<Func<IQueryable<Fact>, IIncludableQueryable<Fact, object>>>()))
            .Returns(Task.FromResult(facts) !);

        _repositoryWrapperMock.SetupSequence(repo => repo.FactRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Fact, Fact>>>(), It.IsAny<Expression<Func<Fact, bool>>>(), default))
            .ReturnsAsync(firstOrDefaultFacts?.FirstOrDefault())
            .ReturnsAsync(firstOrDefaultFacts?.Skip(1).FirstOrDefault())
            .ReturnsAsync(firstOrDefaultFacts?.Skip(2).FirstOrDefault());

        _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(saveChangesResult);

        _mapperMock.Setup(x => x.Map<IEnumerable<FactDto>>(It.IsAny<IEnumerable<Fact>>()))
                  .Returns(returnDto!);
    }

    private IEnumerable<Fact> GetTestFacts()
    {
        return new List<Fact>
        {
            new Fact { Id = 1, Position = 1 },
            new Fact { Id = 2, Position = 2 },
            new Fact { Id = 3, Position = 3 }
        };
    }

    private IEnumerable<FactDto> GetExpectedFactsNew()
    {
        return new List<FactDto>
        {
            new FactDto { Id = 1, Position = 3 },
            new FactDto { Id = 2, Position = 1 },
            new FactDto { Id = 3, Position = 2 }
        };
    }

    private IEnumerable<FactUpdatePositionDto> GetListOfNewPosition()
    {
        return new List<FactUpdatePositionDto>
            {
                new FactUpdatePositionDto { Id = 1, NewPosition = 1 },
                new FactUpdatePositionDto { Id = 2, NewPosition = 2 },
                new FactUpdatePositionDto { Id = 3, NewPosition = 3 }
            };
    }

    private ReorderFactsCommand GetTestReorderFactsCommand()
    {
        return new ReorderFactsCommand(GetListOfNewPosition(), 1);
    }
}