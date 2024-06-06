namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Facts
{
    using System.Collections.Generic;
    using AutoMapper;
    using Moq;
    using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Streetcode.Fact.GetAll;
    using Streetcode.DAL.Entities.Streetcode.TextContent;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Xunit;

    public class GetAllFactsHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> mockRepositoryWrapper;
        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<ILoggerService> mockLogger;
        private readonly List<Fact> facts;
        private readonly List<FactDto> mappedFacts;

        public GetAllFactsHandlerTests()
        {
            this.mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            this.mockMapper = new Mock<IMapper>();
            this.mockLogger = new Mock<ILoggerService>();
            this.facts = new List<Fact> { new Fact { Id = 1, Title = "Test Title", FactContent = "Test Content" } };
            this.mappedFacts = new List<FactDto>() { new FactDto { Id = 1, Title = "Test Title", FactContent = "Test Content" } };
        }

        [Fact]
        public async Task Handle_ReturnError_WhenRepositoryReturnsNull()
        {
            // Arrange
            this.mockRepositoryWrapper
                .Setup(repo => repo.FactRepository.GetAllAsync(default, default))
                .ReturnsAsync((IEnumerable<Fact>)null);
            var handler = new GetAllFactsHandler(this.mockRepositoryWrapper.Object, this.mockMapper.Object, this.mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task Handle_ReturnErrorMessage_WhenRepositoryReturnsNull()
        {
            // Arrange
            this.mockRepositoryWrapper
                .Setup(repo => repo.FactRepository.GetAllAsync(default, default))
                .ReturnsAsync((IEnumerable<Fact>)null);
            var handler = new GetAllFactsHandler(this.mockRepositoryWrapper.Object, this.mockMapper.Object, this.mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

            // Assert
            Assert.Equal("Cannot find any fact", result.Errors.FirstOrDefault()?.Message);
        }

        [Fact]
        public async Task Handle_Success_WhenRepositoryReturnsList()
        {
            // Arrange
            this.mockRepositoryWrapper.
                Setup(repo => repo.FactRepository.GetAllAsync(default, default)).ReturnsAsync(this.facts);
            var handler = new GetAllFactsHandler(this.mockRepositoryWrapper.Object, this.mockMapper.Object, this.mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ReturnsMappedFacts_WhenRepositoryReturnsData()
        {
            // Arrange
            this.mockRepositoryWrapper.
                 Setup(repo => repo.FactRepository.GetAllAsync(default, default)).ReturnsAsync(this.facts);

            this.mockMapper.Setup(mapper => mapper.Map<IEnumerable<FactDto>>(It.IsAny<IEnumerable<Fact>>()))
                .Returns(this.mappedFacts);
            var handler = new GetAllFactsHandler(this.mockRepositoryWrapper.Object, this.mockMapper.Object, this.mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(this.mappedFacts, result.Value);
        }
    }
}
