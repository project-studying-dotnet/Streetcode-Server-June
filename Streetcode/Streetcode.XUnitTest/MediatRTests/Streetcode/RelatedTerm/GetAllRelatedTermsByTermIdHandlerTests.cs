﻿namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.RelatedTerm;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.DAL.Repositories.Interfaces.Base;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

public class GetAllRelatedTermsByTermIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public GetAllRelatedTermsByTermIdHandlerTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task GetAllRelatedTermsByTermId_ShouldReturnError_IfRelatedTermsIsNull()
    {
        // Arrange
        int id = 1;
        MockRepositorySetup(true);

        var handler = new GetAllRelatedTermsByTermIdHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllRelatedTermsByTermIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.HasError(e => e.Message == "Cannot get words by term id"));
    }

    [Fact]
    public async Task GetAllRelatedTermsByTermId_ShouldReturnError_IfRelatedTermsDTOIsNull()
    {
        // Arrange
        int id = 3;
        MockRepositorySetup(false);
        MockMapperSetup(true);

        var handler = new GetAllRelatedTermsByTermIdHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllRelatedTermsByTermIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.HasError(e => e.Message == "Cannot get words by term id"));
    }

    [Fact]
    public async Task GetAllRelatedTermsByTermId_ShouldReturnOk_WhenArgumentsPassedCorrectly()
    {
        // Arrange
        int id = 1;
        MockRepositorySetup(false);
        MockMapperSetup(false);

        var handler = new GetAllRelatedTermsByTermIdHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllRelatedTermsByTermIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAllRelatedTermsByTermId_MapperShouldCallMapOnlyOnce()
    {
        // Arrange
        int id = 1;
        MockRepositorySetup(false);
        MockMapperSetup(false);

        var handler = new GetAllRelatedTermsByTermIdHandler(
            _mockMapper.Object,
            _mockRepository.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetAllRelatedTermsByTermIdQuery(id), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            m => m.Map<IEnumerable<RelatedTermDTO>>(It.IsAny<IEnumerable<Entity>>()),
            Times.Once);
    }

    private static IEnumerable<RelatedTermDTO> GetRelatedTermsDto()
    {
        return new List<RelatedTermDTO> { new() { Id = 1, TermId = 1 }, new() { Id = 2, TermId = 2 } };
    }

    private static IEnumerable<Entity> GetRelatedTerms()
    {
        return new List<Entity> { new() { Id = 1, TermId = 1 }, new() { Id = 2, TermId = 2 } };
    }

    private void MockMapperSetup(bool returnNull)
    {
        _mockMapper
            .Setup(x => x
            .Map<IEnumerable<RelatedTermDTO>>(It.IsAny<IEnumerable<Entity>>()))
            .Returns(returnNull ? null! : GetRelatedTermsDto());
    }

    private void MockRepositorySetup(bool returnNull)
    {
        _mockRepository.Setup(x => x.RelatedTermRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<Entity, bool>>>(),
                    It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync((Expression<Func<Entity, bool>> predicate, Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>> include) =>
            {
                var terms = GetRelatedTerms().AsQueryable();
                if (predicate != null)
                {
                    terms = terms.Where(predicate);
                }
                return returnNull ? new List<Entity>() : terms.ToList();
            });
    }
}