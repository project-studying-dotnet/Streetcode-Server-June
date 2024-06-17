using Xunit;
using Moq;
using Streetcode.BLL.Interfaces.Instagram;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using Streetcode.BLL.MediatR.Instagram.GetAll;
using Streetcode.DAL.Entities.Instagram;
using FluentResults;
using Streetcode.BLL.DTO.Media.Art;

namespace Streetcode.XUnitTest.MediatRTests.Instagram
{
    public class GetAllPostsHandlerTests
    {
        private readonly Mock<IInstagramService> _instagramServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private List<InstagramPost> _instagramPosts = new List<InstagramPost>()
        {
            new InstagramPost()
            {
                Id = "2",
                Caption = "string",
                MediaType = "string",
                MediaUrl = "string",
                Permalink = "string",
                ThumbnailUrl = "string",
                IsPinned = true,
            },
            new InstagramPost()
            {
                Id = "3",
                Caption = "string",
                MediaType = "string",
                MediaUrl = "string",
                Permalink = "string",
                ThumbnailUrl = "string",
                IsPinned = true,
            },
        };
        public GetAllPostsHandlerTests()
        {
            _instagramServiceMock = new Mock<IInstagramService>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_Should_ReturnsAllPosts()
        {
            // Arrange
            _instagramServiceMock.Setup(obj => obj.GetPostsAsync()).ReturnsAsync(_instagramPosts);

            var handle = new GetAllPostsHandler(_instagramServiceMock.Object, _loggerMock.Object);
            var query = new GetAllPostsQuery();

            // Act
            var result = await handle.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_Should_ReturnsFailed()
        {
            // Arrange
            _instagramServiceMock.Setup(obj => obj.GetPostsAsync()).ReturnsAsync(new List<InstagramPost>());

            var handle = new GetAllPostsHandler(_instagramServiceMock.Object, _loggerMock.Object);
            var query = new GetAllPostsQuery();

            // Act
            var result = await handle.Handle(query, CancellationToken.None);
            
            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}
