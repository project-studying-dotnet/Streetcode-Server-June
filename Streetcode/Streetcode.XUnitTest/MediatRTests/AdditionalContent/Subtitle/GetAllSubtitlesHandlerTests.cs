using AutoMapper;
using Castle.Core.Logging;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetAll;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.SubtitleTests
{
    public class GetAllSubtitlesHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repoMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private List<Subtitle> _subtitles = new()
        {
            new Subtitle()
            {
                Id = 1,
                SubtitleText = "string",
                StreetcodeId = 1,
            },
            new Subtitle()
            {
                Id = 2,
                SubtitleText = "string2",
                StreetcodeId = 2,
            },
        };
        public GetAllSubtitlesHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _repoMock = new Mock<IRepositoryWrapper>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResult_WhenRepositoryReturnsSubtitles()
        {
            // Arrange
            _repoMock.Setup(repo => repo.SubtitleRepository.GetAllAsync(default, default)).ReturnsAsync(_subtitles);
            var query = new GetAllSubtitlesQuery();
            var handler = new GetAllSubtitlesHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenRepositoryReturnsNull()
        {
            // Arrange
            _repoMock.Setup(repo => repo.SubtitleRepository.GetAllAsync(default, default)).ReturnsAsync(new List<Subtitle>());
            var query = new GetAllSubtitlesQuery();
            var handler = new GetAllSubtitlesHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}
