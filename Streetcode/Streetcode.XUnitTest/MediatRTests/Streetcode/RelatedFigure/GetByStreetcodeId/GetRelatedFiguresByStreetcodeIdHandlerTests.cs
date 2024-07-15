using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.RelatedFigureTests.GetByStreetcodeId;

public class GetRelatedFiguresByStreetcodeIdHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly GetRelatedFiguresByStreetcodeIdHandler _handler;

    public GetRelatedFiguresByStreetcodeIdHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILoggerService>();
        _blobServiceMock = new Mock<IBlobService>();
        _handler = new GetRelatedFiguresByStreetcodeIdHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _loggerMock.Object, _blobServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenRelatedFiguresAreFound()
    {
        // Arrange
        var streetcodeId = 1;
        var request = new GetRelatedFigureByStreetcodeIdQuery(streetcodeId);
        var relatedFigureIds = new List<int> { 2, 3 };
        var relatedFigures = new List<StreetcodeContent>
        {
            new()
            {
                Id = 2,
                Status = StreetcodeStatus.Published,
                Images = new List<Image> { new Image { BlobName = "blob1", ImageDetails = new ImageDetails { Alt = "a" } } }
            },
            new()
            {
                Id = 3,
                Status = StreetcodeStatus.Published,
                Images = new List<Image> { new Image { BlobName = "blob2", ImageDetails = new ImageDetails { Alt = "b" } } }
            }
        };

        var relatedFiguresDto = new List<RelatedFigureDTO>
        {
            new()
            {
                Id = 2,
                Images = new List<ImageDTO> { new ImageDTO { BlobName = "blob1" } }
            },
            new()
            {
                Id = 3,
                Images = new List<ImageDTO> { new ImageDTO { BlobName = "blob2" } }
            }
        };

        _repositoryWrapperMock.Setup(x => x.RelatedFigureRepository.FindAll(It.IsAny<Expression<Func<RelatedFigure, bool>>>()))
               .Returns((Expression<Func<RelatedFigure, bool>> predicate) =>
                   relatedFigureIds.Select(id => new RelatedFigure { ObserverId = id, TargetId = id }).AsQueryable());

        _repositoryWrapperMock.Setup(x => x.StreetcodeRepository.GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(relatedFigures);

        _mapperMock.Setup(x => x.Map<IEnumerable<RelatedFigureDTO>>(relatedFigures)).Returns(relatedFiguresDto);

        _blobServiceMock.Setup(x => x.FindFileInStorageAsBase64(It.IsAny<string>())).Returns("base64string");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
        Assert.Equal(streetcodeId, result.Value.First().CurrentStreetcodeId);
        Assert.Equal(streetcodeId, result.Value.Last().CurrentStreetcodeId);
        _blobServiceMock.Verify(x => x.FindFileInStorageAsBase64(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenRelatedFigureIdsNotFound()
    {
        // Arrange
        var streetcodeId = 1;
        var request = new GetRelatedFigureByStreetcodeIdQuery(streetcodeId);

        _repositoryWrapperMock.Setup(r => r.RelatedFigureRepository.FindAll(It.IsAny<Expression<Func<RelatedFigure, bool>>>()))
            .Returns(Enumerable.Empty<RelatedFigure>().AsQueryable());

        _repositoryWrapperMock.Setup(r => r.StreetcodeRepository
        .GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                              .ReturnsAsync(It.IsAny<List<StreetcodeContent>>);
        var expectedMessage = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFoundWithStreetcode, request);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenRelatedFiguresNotFound()
    {
        // Arrange
        var streetcodeId = 1;
        var request = new GetRelatedFigureByStreetcodeIdQuery(streetcodeId);

        _repositoryWrapperMock.Setup(r => r.RelatedFigureRepository.FindAll(It.IsAny<Expression<Func<RelatedFigure, bool>>>()))
            .Returns(new List<RelatedFigure> { new() { ObserverId = streetcodeId, TargetId = 2 } }.AsQueryable());

        _repositoryWrapperMock.Setup(r => r.StreetcodeRepository
            .GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(new List<StreetcodeContent>());

        var expectedMessage = MessageResourceContext.GetMessage(ErrorMessages.EntityWithStreetcodeNotFound, request);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedMessage, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenMapperFails()
    {
        // Arrange
        var streetcodeId = 1;
        var request = new GetRelatedFigureByStreetcodeIdQuery(streetcodeId);

        var relatedFigures = new List<StreetcodeContent>
        {
            new()
            {
                Id = 2,
                Status = StreetcodeStatus.Published,
                Images = new List<Image> { new Image { BlobName = "blob1", ImageDetails = new ImageDetails { Alt = "a" } } }
            }
        };

        _repositoryWrapperMock.Setup(r => r.RelatedFigureRepository.FindAll(It.IsAny<Expression<Func<RelatedFigure, bool>>>()))
            .Returns(new List<RelatedFigure> { new() { ObserverId = streetcodeId, TargetId = 2 } }.AsQueryable());

        _repositoryWrapperMock.Setup(r => r.StreetcodeRepository
            .GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(relatedFigures);

        _mapperMock.Setup(x => x.Map<IEnumerable<RelatedFigureDTO>>(relatedFigures))
            .Returns((IEnumerable<RelatedFigureDTO>)null!);

        var expectedMessage = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedMessage, result.Errors.First().Message);
        _loggerMock.Verify(x => x.LogError(request, expectedMessage), Times.Once);
    }
}