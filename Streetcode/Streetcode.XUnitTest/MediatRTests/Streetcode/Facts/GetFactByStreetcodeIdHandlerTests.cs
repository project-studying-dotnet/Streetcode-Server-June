﻿// <copyright file="GetFactByStreetcodeIdHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Facts
{
    using System.Linq.Expressions;
    using AutoMapper;
    using Moq;
    using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;
    using Streetcode.BLL.Resources;
    using Streetcode.DAL.Entities.Streetcode.TextContent;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Xunit;

    public class GetFactByStreetcodeIdHandlerTests
    {
        private const string ERRORMESSAGE = "Cannot find any Fact by a streetcode id: ";

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
            var fact = _facts[0];
            var request = new GetFactByStreetcodeIdQuery(fact.StreetcodeId);
            _mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllAsync(It.IsAny<Expression<Func<Fact, bool>>>(), default)).ReturnsAsync((IEnumerable<Fact>)null!);
            var handler = new GetFactByStreetcodeIdHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);
            var expectedErrorMessage = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFoundWithStreetcode, request);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.IsFailed);
                Assert.Equal(expectedErrorMessage, result.Errors[0].Message);
            });
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