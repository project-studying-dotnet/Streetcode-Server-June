namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Facts;

using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

public class GetFactByIdHandlerTest
{
    private const string ERRORMESSAGE = "Cannot find any fact with corresponding id: ";

    private readonly Mock<IRepositoryWrapper> mockRepositoryWrapper;
    private readonly Mock<IMapper> mockMapper;
    private readonly Mock<ILoggerService> mockLogger;
    private readonly List<Fact> facts;
    private readonly List<FactDto> mappedFacts;

    public GetFactByIdHandlerTest()
    {
        mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        mockMapper = new Mock<IMapper>();
        mockLogger = new Mock<ILoggerService>();
        facts = new List<Fact>
        {
            new Fact { Id = 1, Title = "Test Title", FactContent = "Test Content" },
            new Fact { Id = 2, Title = "Test Title2", FactContent = "Test Content2" },
        };
        mappedFacts = new List<FactDto>()
        {
            new FactDto { Id = 1, Title = "Test Title", FactContent = "Test Content" },
            new FactDto { Id = 2 },
        };
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenRepositoryHasCorrectParameters()
    {
        // Arrange
        Fact fact = facts[0];
        Fact otherFact = facts[1];

        mockRepositoryWrapper.
            Setup(repo => repo.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                default))
            .ReturnsAsync(fact);
        var handler = new GetFactByIdHandler(
            mockRepositoryWrapper.Object,
            mockMapper.Object,
            mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetFactByIdQuery(fact.Id), CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => Assert.True(result.IsSuccess),
            () => mockRepositoryWrapper.Verify(repo => repo.FactRepository.GetFirstOrDefaultAsync(
                It.Is<Expression<Func<Fact, bool>>>(predicate => predicate.Compile().Invoke(fact)),
                default)),
            () => mockRepositoryWrapper.Verify(repo => repo.FactRepository.GetFirstOrDefaultAsync(
                It.Is<Expression<Func<Fact, bool>>>(predicate => !predicate.Compile().Invoke(otherFact)),
                default)));
    }

    [Fact]
    public async Task Handle_Should_ReturnMappedFacts_WhenRepositoryReturnsData()
    {
        // Arrange
        Fact fact = facts[0];
        FactDto mappedFact = mappedFacts[0];

        mockRepositoryWrapper.
             Setup(repo => repo.FactRepository
             .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Fact, bool>>>(), default))
             .ReturnsAsync(fact);
        mockMapper.Setup(mapper => mapper.Map<FactDto>(fact))
            .Returns(mappedFact);

        var handler = new GetFactByIdHandler(
            mockRepositoryWrapper.Object,
            mockMapper.Object,
            mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetFactByIdQuery(fact.Id), CancellationToken.None);

        // Assert
        Assert.Multiple(
       () => Assert.True(result.IsSuccess),
       () => Assert.Equal(mappedFact.Id, result.Value.Id));
    }

    [Fact]
    public async Task Handle_Should_ReturnErrorMessage_WhenRepositoryReturnsNull()
    {
        // Arrange
        mockRepositoryWrapper
            .Setup(repo => repo.FactRepository
            .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Fact, bool>>>(), default))
            .ReturnsAsync((Fact)null!);

        var handler = new GetFactByIdHandler(
            mockRepositoryWrapper.Object,
            mockMapper.Object,
            mockLogger.Object);

        // Act
        var result = await handler.Handle(
            new GetFactByIdQuery(facts[0].Id),
            CancellationToken.None);

        // Assert
        Assert.Multiple(
        () => Assert.True(result.IsFailed),
        () => Assert.Equal($"{ERRORMESSAGE}{facts[0].Id}", result.Errors.FirstOrDefault()?.Message));
    }
}