using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.Media.Art
{
    public class GetAllArtsHandlerTest
    {
        private readonly Mock<IRepositoryWrapper> mockRepo;
        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<ILoggerService> mockLogger;

        public GetAllArtsHandlerTest()
        {
            mockRepo = new Mock<IRepositoryWrapper>();
            mockMapper = new Mock<IMapper>();
            mockLogger = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_Should_ReturnsAllArts()
        {
            // Arrange
            var mockHandler = CreateHandler(GetArtsList(), GetArtsDtoList());

            // Act
            var result = await mockHandler.Handle(new GetAllArtsQuery(), default);

            // Assert
            Assert.Equal(GetArtsList().Count, result.Value.Count());
        }

        [Fact]
        public async Task Handle_Should_ReturnsZeroArts_WhenNotExists()
        {
            // Arrange
            var mockHandler = CreateHandler(new List<DAL.Entities.Media.Images.Art>(), new List<ArtDTO>());

            // Act
            var result = await mockHandler.Handle(new GetAllArtsQuery(), default);

            // Assert
            Assert.Equal(0, result.Value.Count());
        }

        [Fact]
        public async Task Handle_Should_ReturnsDtoType()
        {
            // Arrange
            var mockHandler = CreateHandler(GetArtsList(), GetArtsDtoList());

            // Act
            var result = await mockHandler.Handle(new GetAllArtsQuery(), CancellationToken.None);

            // Assert
            Assert.IsType<Result<IEnumerable<ArtDTO>>>(result);
        }

        private GetAllArtsHandler CreateHandler(IEnumerable<DAL.Entities.Media.Images.Art> artList, IEnumerable<ArtDTO> artListDto)
        {
            MockRepository(artList);
            MockMapper(artListDto);

            return new GetAllArtsHandler(mockRepo.Object, mockMapper.Object, mockLogger.Object);
        }

        private static List<DAL.Entities.Media.Images.Art> GetArtsList() => new() { new DAL.Entities.Media.Images.Art { Id = 1, Title = "Title 1" }, new DAL.Entities.Media.Images.Art { Id = 2, Title = "Title 2" } };

        private static List<ArtDTO> GetArtsDtoList() => new() { new ArtDTO { Id = 11, Title = "TitleDto 1" }, new ArtDTO { Id = 22, Title = "TitleDto 2" } };

        private void MockRepository(IEnumerable<DAL.Entities.Media.Images.Art> artList)
        {
            mockRepo.Setup(repo => repo.ArtRepository.GetAllAsync(
                It.IsAny<Expression<Func<DAL.Entities.Media.Images.Art, bool>>>(), It.IsAny<Func<IQueryable<DAL.Entities.Media.Images.Art>, IIncludableQueryable<DAL.Entities.Media.Images.Art, object>>>()))
                    .ReturnsAsync(artList);
        }

        private void MockMapper(IEnumerable<ArtDTO> artListDto)
        {
            mockMapper.Setup(mapper => mapper.Map<IEnumerable<ArtDTO>>(It.IsAny<IEnumerable<object>>())).Returns(artListDto);
        }
    }
}
