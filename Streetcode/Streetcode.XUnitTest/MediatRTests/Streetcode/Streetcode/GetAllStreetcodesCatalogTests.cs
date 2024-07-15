using Ardalis.Specification;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTest.StreetcodeBlock;

public class GetAllStreetcodesCatalogTests
{
    private const int PAGE = 1;
    private const int COUNT = 10;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly IEnumerable<StreetcodeContent> _list;
    private readonly IEnumerable<RelatedFigureDTO> _mappedList;

    private readonly GetAllStreetcodesCatalogHandler _handler;
    private readonly GetAllStreetcodesCatalogQuery _query;

    public GetAllStreetcodesCatalogTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();

        _handler = new GetAllStreetcodesCatalogHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
        _query = new GetAllStreetcodesCatalogQuery(PAGE, COUNT);
        _list = new List<StreetcodeContent> { new StreetcodeContent { Id = 1, Title = "Test" } };
        _mappedList = new List<RelatedFigureDTO> { new RelatedFigureDTO { Id = 1, Title = "Test" } };

        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<RelatedFigureDTO>>(_list))
            .Returns(_mappedList);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenRepositoryHasCorrectParameters()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository
            .GetAllWithSpecAsync(It.IsAny<ISpecification<StreetcodeContent>[]>()))
            .ReturnsAsync(_list);

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        Assert.Multiple(
       () => Assert.True(result.IsSuccess),
       () => _mockRepositoryWrapper.Verify(repo => repo.StreetcodeRepository.GetAllWithSpecAsync(
                It.IsAny<ISpecification<StreetcodeContent>[]>())));
    }

    [Fact]
    public async Task Handle_Should_ReturnMappedRelatedFigureDTO_WhenRepositoryReturnsData()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository
            .GetAllWithSpecAsync(It.IsAny<ISpecification<StreetcodeContent>[]>()))
            .ReturnsAsync(_list);

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        Assert.Multiple(
       () => Assert.True(result.IsSuccess),
       () => Assert.Equal(_mappedList, result.Value));
    }

    [Fact]
    public async Task Handle_Should_ReturnErrorMessage_WhenRepositoryReturnsNull()
    {
        // Arrange
        _mockRepositoryWrapper.Setup(repo => repo.StreetcodeRepository
            .GetAllWithSpecAsync(It.IsAny<ISpecification<StreetcodeContent>[]>()))
            .ReturnsAsync((IEnumerable<StreetcodeContent>)null!);

        // Act
        var expectedErrorMessage = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, _query);
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        Assert.Multiple(
       () => Assert.True(result.IsFailed),
       () => Assert.Equal(expectedErrorMessage, result.Errors.FirstOrDefault()?.Message));
    }
}
