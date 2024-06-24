using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Media.Art
{
    public class CreateArtHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly CreateArtHandler _handler;

        public CreateArtHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new CreateArtHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenArtIsNull()
        {
            // Arrange
            var request = new CreateArtCommand(new ArtCreateUpdateDTO());
            _mapperMock.Setup(m => m.Map<DAL.Entities.Media.Images.Art>(It.IsAny<ArtCreateUpdateDTO>())).Returns((DAL.Entities.Media.Images.Art)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenSaveChangesFails()
        {
            // Arrange
            var artEntity = new DAL.Entities.Media.Images.Art();
            var request = new CreateArtCommand(new ArtCreateUpdateDTO());

            _mapperMock.Setup(m => m.Map<DAL.Entities.Media.Images.Art>(It.IsAny<ArtCreateUpdateDTO>())).Returns(artEntity);
            _repositoryWrapperMock.Setup(r => r.ArtRepository.CreateAsync(It.IsAny<DAL.Entities.Media.Images.Art>())).ReturnsAsync(artEntity);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkResult_WhenArtIsCreatedSuccessfully()
        {
            // Arrange
            var artEntity = new DAL.Entities.Media.Images.Art();
            var request = new CreateArtCommand(new ArtCreateUpdateDTO());

            _mapperMock.Setup(m => m.Map<DAL.Entities.Media.Images.Art>(It.IsAny<ArtCreateUpdateDTO>())).Returns(artEntity);
            _repositoryWrapperMock.Setup(r => r.ArtRepository.CreateAsync(It.IsAny<DAL.Entities.Media.Images.Art>())).ReturnsAsync(artEntity);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<ArtCreateUpdateDTO>(It.IsAny<DAL.Entities.Media.Images.Art>())).Returns(request.Art);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenArtIsNull()
        {
            // Arrange
            var request = new CreateArtCommand(new ArtCreateUpdateDTO());
            _mapperMock.Setup(m => m.Map<DAL.Entities.Media.Images.Art>(It.IsAny<ArtCreateUpdateDTO>())).Returns((DAL.Entities.Media.Images.Art)null);
            const string expectedErrorMsg = "Cannot convert null to Art";

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Contains(result.Errors, e => e.Message == expectedErrorMsg);
        }
    }
}
