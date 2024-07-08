namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.RelatedTerm;

using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;

using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

public class GetRelatedTermByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetRelatedTermByIdHandler _handler;

    public GetRelatedTermByIdHandlerTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetRelatedTermByIdHandler(_mockMapper.Object, _mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRelatedTermNotFound()
    {
        // Arrange
        var request = new GetRelatedTermByIdQuery(1);
        _mockRepository.Setup(x => x.RelatedTermRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync((Entity?)null);
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, request);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
        });
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRelatedTermFound()
    {
        // Arrange
        var relatedTerm = new Entity { Id = 1, Word = "Test", TermId = 1 };
        var relatedTermDto = new RelatedTermDTO { Id = 1, Word = "Test", TermId = 1 };

        _mockRepository.Setup(x => x.RelatedTermRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(relatedTerm);
        _mockMapper.Setup(mapper => mapper.Map<RelatedTermDTO>(relatedTerm)).Returns(relatedTermDto);

        // Act
        var result = await _handler.Handle(new GetRelatedTermByIdQuery(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(relatedTermDto, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenMappingFails()
    {
        // Arrange
        var relatedTerm = new Entity { Id = 1, Word = "Test", TermId = 1 };
        var request = new GetRelatedTermByIdQuery(1);

        _mockRepository.Setup(x => x.RelatedTermRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(relatedTerm);
        _mockMapper.Setup(mapper => mapper.Map<RelatedTermDTO>(relatedTerm)).Returns((RelatedTermDTO)null);
        var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
        });
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryAndMapperCorrectly()
    {
        // Arrange
        var relatedTerm = new Entity { Id = 1, Word = "Test", TermId = 1 };
        var relatedTermDto = new RelatedTermDTO { Id = 1, Word = "Test", TermId = 1 };

        _mockRepository.Setup(x => x.RelatedTermRepository
                .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
            .ReturnsAsync(relatedTerm);
        _mockMapper.Setup(mapper => mapper.Map<RelatedTermDTO>(relatedTerm)).Returns(relatedTermDto);

        // Act
        await _handler.Handle(new GetRelatedTermByIdQuery(1), CancellationToken.None);

        // Assert
        _mockRepository.Verify(x => x.RelatedTermRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()), Times.Once);
        _mockMapper.Verify(x => x.Map<RelatedTermDTO>(relatedTerm), Times.Once);
    }
}
