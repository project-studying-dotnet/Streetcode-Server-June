using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specification.Media.ArtSpec.GetByStreetcode;
using Streetcode.BLL.Specification.Streetcode.Streetcode.GetByFilter;
using Streetcode.BLL.Specification.Streetcode.TextContent.FactSpec.GetAll;
using Streetcode.BLL.Specification.Streetcode.TextSec.GetAll;
using Streetcode.BLL.Specification.TimeLine;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using Streetcode.BLL.DTO.AdditionalContent.Filter;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.StreetcodeTest;

public class GetStreetcodeByFilterHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetStreetcodeByFilterHandler _handler;

    public GetStreetcodeByFilterHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILoggerService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetStreetcodeByFilterHandler(_repositoryWrapperMock.Object, _loggerMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_EmptySearchQuery_ReturnsFailResult()
    {
        // Arrange
        var request = CreateQuery(string.Empty);
        var cancellationToken = CancellationToken.None;
        SetupMapper();

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ValidSearchQuery_ReturnsSuccessResult()
    {
        // Arrange
        var request = CreateQuery("valid-query");
        var cancellationToken = CancellationToken.None;

        SetupRepositories(
            streetcodeResults: new List<StreetcodeContent> { new StreetcodeContent() },
            textResults: new List<DAL.Entities.Streetcode.TextContent.Text> { new DAL.Entities.Streetcode.TextContent.Text() },
            factResults: new List<Fact> { new Fact() },
            timelineResults: new List<TimelineItem> { new TimelineItem() });

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        Assert.Equal(5, result.Value.Count); 
    }

    [Fact]
    public async Task Handle_ArtResultsWithNullStreetcode_ExcludesNullStreetcode()
    {
        // Arrange
        var request = CreateQuery("valid-query");
        var cancellationToken = CancellationToken.None;

        var artResults = CreateArtResultsWithNullStreetcode();

        SetupRepositories(artResults: artResults);
        SetupMapper();

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        Assert.Equal(1, result.Value?.Count);
    }

    private void SetupRepositories(
      List<StreetcodeContent>? streetcodeResults = null,
      List<DAL.Entities.Streetcode.TextContent.Text>? textResults = null,
      List<Fact>? factResults = null,
      List<TimelineItem>? timelineResults = null,
      List<Art>? artResults = null)
    {
        streetcodeResults ??= new List<StreetcodeContent>();
        textResults ??= new List<DAL.Entities.Streetcode.TextContent.Text>();
        factResults ??= new List<Fact>();
        timelineResults ??= new List<TimelineItem>();
        artResults ??= new List<Art>()
        {
            new ()
            {
                StreetcodeArts = new List<StreetcodeArt>
                    {
                        new StreetcodeArt { Streetcode = new StreetcodeContent() },
                    }
            }
        };

        _repositoryWrapperMock.Setup(repo => repo.StreetcodeRepository.GetAllWithSpecAsync(It.IsAny<StreetcodesFilteredByQuerySpec>()))
            .ReturnsAsync(streetcodeResults);

        _repositoryWrapperMock.Setup(repo => repo.TextRepository.GetAllWithSpecAsync(It.IsAny<TextFilteredByQuerySpec>()))
            .ReturnsAsync(textResults);

        _repositoryWrapperMock.Setup(repo => repo.FactRepository.GetAllWithSpecAsync(It.IsAny<FactsFilteredByQuerySpec>()))
            .ReturnsAsync(factResults);

        _repositoryWrapperMock.Setup(repo => repo.TimelineRepository.GetAllWithSpecAsync(It.IsAny<TimeLinesIncludePublishStreetcodeSpec>()))
            .ReturnsAsync(timelineResults);

        _repositoryWrapperMock.Setup(repo => repo.ArtRepository.GetAllWithSpecAsync(It.IsAny<ArtsFilteredByQuerySpec>()))
            .ReturnsAsync(artResults);

        SetupMapper();
    }

    private void SetupMapper()
    {
        _mapperMock.Setup(mapper => mapper.Map<StreetcodeFilterResultDTO>(It.IsAny<object>()))
            .Returns(new StreetcodeFilterResultDTO());
    }

    private List<Art> CreateArtResultsWithNullStreetcode()
    {
        return new List<Art>
            {
                new Art
                {
                    StreetcodeArts = new List<StreetcodeArt>
                    {
                        new StreetcodeArt { Streetcode = null },
                        new StreetcodeArt { Streetcode = new StreetcodeContent() }
                    }
                }
            };
    }

    private GetStreetcodeByFilterQuery CreateQuery(string searchQuery)
    {
        var filter = new StreetcodeFilterRequestDTO { SearchQuery = searchQuery };
        return new GetStreetcodeByFilterQuery(filter);
    }
}