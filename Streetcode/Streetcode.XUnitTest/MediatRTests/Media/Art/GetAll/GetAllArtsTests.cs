using Moq;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using AutoMapper;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using FluentResults;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;

namespace Streetcode.XUnitTest.MediatRTests.Media.Arts
{
    public class GetAllArtsTest
    {
        private readonly Mock<IRepositoryWrapper> mockRepo;
        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<ILoggerService> mockLogger;

        public GetAllArtsTest()
        {
            mockRepo = new Mock<IRepositoryWrapper>();
            mockMapper = new Mock<IMapper>();
            mockLogger = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_ReturnsAllArts()
        {
            // Arrange
            var mockHandler = CreateHandler(GetArtsList(), GetArtsDTOList());

            // Act
            var result = await mockHandler.Handle(new GetAllArtsQuery(), default);

            // Assert
            Assert.Equal(GetArtsList().Count, result.Value.Count());
        }

        [Fact]
        public async Task Handle_ReturnsZero()
        {
            // Arrange
            var mockHandler = CreateHandler(new List<Art>(), new List<ArtDTO>());

            // Act
            var result = await mockHandler.Handle(new GetAllArtsQuery(), default);

            // Assert
            Assert.Equal(0, result.Value.Count());
        }

        [Fact]
        public async Task Handle_ReturnsType()
        {
            // Arrange
            var mockHandler = CreateHandler(GetArtsList(), GetArtsDTOList());

            // Act
            var result = await mockHandler.Handle(new GetAllArtsQuery(), CancellationToken.None);

            // Assert
            Assert.IsType<Result<IEnumerable<ArtDTO>>>(result);
        }

        private GetAllArtsHandler CreateHandler(List<Art> artList, List<ArtDTO> artListDTO)
        {
            MockRepository(artList);
            MockMapper(artListDTO);

            return new GetAllArtsHandler(mockRepo.Object, mockMapper.Object, mockLogger.Object);
        }

        private List<Art> GetArtsList() => new List<Art> { new Art { Id = 1, Title = "Title 1" }, new Art { Id = 2, Title = "Title 2" } };

        private List<ArtDTO> GetArtsDTOList() => new List<ArtDTO> { new ArtDTO { Id = 1, Title = "Title 1" }, new ArtDTO { Id = 2, Title = "Title 2" } };

        private void MockRepository(List<Art> artList)
        {
            mockRepo.Setup(repo => repo.ArtRepository.GetAllAsync(
                It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>()))
                    .ReturnsAsync(artList);
        }

        private void MockMapper(List<ArtDTO> artListDTO)
        {
            mockMapper.Setup(mapper => mapper.Map<IEnumerable<ArtDTO>>(It.IsAny<IEnumerable<object>>())).Returns(artListDTO);
        }
    }

}