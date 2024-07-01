namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.RelatedTerm;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

public class GetAllRelatedTermsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetAllRelatedTermsHandler _handler;

    public GetAllRelatedTermsHandlerTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetAllRelatedTermsHandler(_mockMapper.Object, _mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResultWithRelatedTerms()
    {
        // Arrange
        var relatedTerms = GetRelatedTerms();
        var relatedTermsDto = GetRelatedTermDTOs();
        MockRepositorySetup(relatedTerms);
        MockMapperSetup(relatedTerms, relatedTermsDto);

        // Act
        var result = await _handler.Handle(new GetAllRelatedTermsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(relatedTermsDto.Count, result.Value.Count());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyResult_WhenNoRelatedTermsExist()
    {
        // Arrange
        MockRepositorySetup(new List<Entity>());
        MockMapperSetup(new List<Entity>(), new List<RelatedTermDTO>());

        // Act
        var result = await _handler.Handle(new GetAllRelatedTermsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldCallMapperCorrectly()
    {
        // Arrange
        var relatedTerms = GetRelatedTerms();
        var relatedTermsDto = GetRelatedTermDTOs();
        MockRepositorySetup(relatedTerms);
        MockMapperSetup(relatedTerms, relatedTermsDto);

        // Act
        await _handler.Handle(new GetAllRelatedTermsQuery(), CancellationToken.None);

        // Assert
        _mockMapper.Verify(m => m.Map<IEnumerable<RelatedTermDTO>>(relatedTerms), Times.Once);
    }

    private static List<RelatedTermDTO> GetRelatedTermDTOs()
    {
        return new List<RelatedTermDTO>
        {
            new RelatedTermDTO { Id = 1, Word = "Test1" },
            new RelatedTermDTO { Id = 2, Word = "Test2" }
        };
    }

    private static List<Entity> GetRelatedTerms()
    {
        return new List<Entity>
        {
            new Entity { Id = 1, Word = "Test1", TermId = 1 },
            new Entity { Id = 2, Word = "Test2", TermId = 2 }
        };
    }

    private void MockMapperSetup(IEnumerable<Entity> relatedTerms, IEnumerable<RelatedTermDTO> relatedTermsDto)
    {
        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<RelatedTermDTO>>(relatedTerms)).Returns(relatedTermsDto);
    }

    private void MockRepositorySetup(IEnumerable<Entity> relatedTerms)
    {
        _mockRepository.Setup(repo => repo.RelatedTermRepository
            .GetAllAsync(It.IsAny<Expression<Func<Entity, bool>>>(),
                         It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(relatedTerms);
    }
}
