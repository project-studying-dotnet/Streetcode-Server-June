using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.Update;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Media.Art
{
    public class UpdateArtHandlerTest
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly UpdateArtHandler _handler;

        public UpdateArtHandlerTest()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new UpdateArtHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenArtIsUpdated()
        {
            var artDto = new ArtCreateUpdateDTO { Id = 1, Title = "Art Title" };
            var art = new DAL.Entities.Media.Images.Art { Id = 1, Title = "Art Title" };

            var command = new UpdateArtCommand(artDto);

            _mapperMock.Setup(m => m.Map<DAL.Entities.Media.Images.Art>(artDto)).Returns(art);
            _repositoryWrapperMock.Setup(r => r.ArtRepository.Update(art));
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenArtIsNull()
        {
            // Arrange
            var artDto = new ArtCreateUpdateDTO();
            _mapperMock.Setup(m => m.Map<DAL.Entities.Media.Images.Art>(artDto)).Returns<DAL.Entities.Media.Images.Art>(null);
            var request = new UpdateArtCommand(artDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnErrorMsg_WhenArtIsNull()
        {
            // Arrange
            var artDto = new ArtCreateUpdateDTO();
            _mapperMock.Setup(m => m.Map<DAL.Entities.Media.Images.Art>(artDto)).Returns<DAL.Entities.Media.Images.Art>(null);
            var request = new UpdateArtCommand(artDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal("Cannot convert null to Art", result.Errors[0].Message);
        }
    }
}
