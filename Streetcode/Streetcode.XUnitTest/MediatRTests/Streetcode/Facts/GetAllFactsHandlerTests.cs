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
        private const string ERRORMESSAGE = "Cannot find any Fact";

        private readonly Mock<IRepositoryWrapper> mockRepositoryWrapper;
        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<ILoggerService> mockLogger;
        private readonly List<Fact> facts;
        private readonly List<FactDto> mappedFacts;

        public GetAllFactsHandlerTests()
        {
            mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            mockMapper = new Mock<IMapper>();
            mockLogger = new Mock<ILoggerService>();
            facts = new List<Fact> { new Fact { Id = 1, Title = "Test Title", FactContent = "Test Content" } };
            mappedFacts = new List<FactDto>() { new FactDto { Id = 1, Title = "Test Title", FactContent = "Test Content" } };
        }

        [Fact]
        public async Task Handle_Should_ReturnErrorMessage_WhenRepositoryReturnsNull()
        {
            // Arrange
            mockRepositoryWrapper
                .Setup(repo => repo.FactRepository.GetAllAsync(default, default))
                .ReturnsAsync((IEnumerable<Fact>)null!);
            var handler = new GetAllFactsHandler(mockRepositoryWrapper.Object, mockMapper.Object, mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(result.IsFailed),
                () => Assert.Equal(ERRORMESSAGE, result.Errors.FirstOrDefault()?.Message));
        }

        [Fact]
        public async Task Handle_Should_ReturnsMappedFacts_WhenRepositoryReturnsData()
        {
            // Arrange
            mockRepositoryWrapper.
                 Setup(repo => repo.FactRepository.GetAllAsync(default, default)).ReturnsAsync(facts);

            mockMapper.Setup(mapper => mapper.Map<IEnumerable<FactDto>>(It.IsAny<IEnumerable<Fact>>()))
                .Returns(mappedFacts);
            var handler = new GetAllFactsHandler(mockRepositoryWrapper.Object, mockMapper.Object, mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(result.IsSuccess),
                () => Assert.Equal(mappedFacts, result.Value));
        }
    }
}