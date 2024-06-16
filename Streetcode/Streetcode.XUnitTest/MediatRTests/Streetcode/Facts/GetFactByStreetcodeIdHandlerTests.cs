﻿using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Facts
{
    public class GetFactByStreetcodeIdHandlerTests
    {
        private const string ERRORMESSAGE = "Cannot find any fact by the streetcode id: ";

        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly List<Fact> _facts;
        private readonly List<FactDto> _mappedFacts;

        public GetFactByStreetcodeIdHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
            _facts = new List<Fact>
            {
                new Fact { Id = 1, Title = "Test Title", FactContent = "Test Content", StreetcodeId = 1 },
                new Fact { Id = 2, Title = "Test Title2", FactContent = "Test Content2", StreetcodeId = 1 },
                new Fact { Id = 3, Title = "Test Title2", FactContent = "Test Content2", StreetcodeId = 2 },
            };
            _mappedFacts = new List<FactDto>()
            {
                new FactDto { Id = 1, Title = "Test Title", FactContent = "Test Content" },
                new FactDto { Id = 2 },
            };
        }

        [Fact]
        public async Task Handle_Should_Success_WhenRepositoryHasCorrectParameters()
        {
            // Arrange
            Fact fact = _facts[0];
            Fact fact1 = _facts[1];
            Fact otherFact = _facts[2];

            MockingWrapperAndMapperWithValue();

            var handler = new GetFactByStreetcodeIdHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetFactByStreetcodeIdQuery(fact.StreetcodeId), CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(result.IsSuccess),
                () => _mockRepositoryWrapper.Verify(repo => repo.FactRepository.GetAllAsync(
                    It.Is<Expression<Func<Fact, bool>>>(predicate => predicate.Compile().Invoke(fact)),
                    default)),
                () => _mockRepositoryWrapper.Verify(repo => repo.FactRepository.GetAllAsync(
                    It.Is<Expression<Func<Fact, bool>>>(predicate => predicate.Compile().Invoke(fact1)),
                    default)),
                () => _mockRepositoryWrapper.Verify(repo => repo.FactRepository.GetAllAsync(
                    It.Is<Expression<Func<Fact, bool>>>(predicate => !predicate.Compile().Invoke(otherFact)),
                    default)));
        }

        [Fact]
        public async Task Handle_Should_ReturnsMappedFacts_WhenRepositoryReturnsData()
        {
            // Arrange
            MockingWrapperAndMapperWithValue();

            var handler = new GetFactByStreetcodeIdHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetFactByStreetcodeIdQuery(_facts[0].StreetcodeId), CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(result.IsSuccess),
                () => Assert.Equal(_mappedFacts, result.Value));
        }

        [Fact]
        public async Task Handle_Should_ReturnErrorMessage_WhenRepositoryReturnsNull()
        {
            // Arrange
            Fact fact = _facts[0];

            _mockRepositoryWrapper
                .Setup(repo => repo.FactRepository.GetAllAsync(
                    It.IsAny<Expression<Func<Fact, bool>>>(), default))
                .ReturnsAsync((IEnumerable<Fact>)null!);

            var handler = new GetFactByStreetcodeIdHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(
                new GetFactByStreetcodeIdQuery(fact.StreetcodeId),
                CancellationToken.None);

            // Assert
            Assert.Multiple(
                () => Assert.True(result.IsFailed),
                () => Assert.Equal($"{ERRORMESSAGE}{fact.StreetcodeId}", result.Errors.FirstOrDefault()?.Message));
        }

        private void MockingWrapperAndMapperWithValue()
        {
            _mockRepositoryWrapper.
                 Setup(repo => repo.FactRepository.GetAllAsync(default, default)).ReturnsAsync(_facts);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<FactDto>>(It.IsAny<IEnumerable<Fact>>()))
                .Returns(_mappedFacts);
        }
    }
}
