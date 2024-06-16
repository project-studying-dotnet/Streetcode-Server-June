namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.RelatedTerm;

using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using Entity = Streetcode.DAL.Entities.Streetcode.TextContent.RelatedTerm;

    public class CreateRelatedTermHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;

        public CreateRelatedTermHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _mockMapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenRelatedTermIsNull()
        {
            // Arrange
            MockRepositorySetupNullOrEmptyArrOffIds();
            var command = new CreateRelatedTermCommand(null!);
            var handler = new CreateRelatedTermHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Cannot create new related word for a term!", result.Errors.FirstOrDefault()?.Message);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenRelatedTermAlreadyExists()
        {
            // Arrange
            var request = GetValidCreateRelatedTermRequest();
            SetupMockForExistingTerm(request);
            var handler = CreateHandler();
            var command = new CreateRelatedTermCommand(request);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Word already exists for this term!", result.Errors.FirstOrDefault()?.Message);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenSaveChangesFails()
        {
            // Arrange
            var request = GetValidCreateRelatedTermRequest();
            SetupMockForSaveChangesFail(request);
            var handler = CreateHandler();
            var command = new CreateRelatedTermCommand(request);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Cannot save changes in the database after related word creation!", result.Errors.FirstOrDefault()?.Message);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenMappingFails()
        {
            // Arrange
            var request = GetValidCreateRelatedTermRequest();
            SetupMockForMappingFail(request);
            var handler = CreateHandler();
            var command = new CreateRelatedTermCommand(request);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Cannot map entity!", result.Errors.FirstOrDefault()?.Message);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResult_WhenOperationIsSuccessful()
        {
            // Arrange
            var request = GetValidCreateRelatedTermRequest();
            SetupMockForSuccess(request);
            var handler = CreateHandler();
            var command = new CreateRelatedTermCommand(request);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Value.Should().BeEquivalentTo(request);
        }

        private CreateRelatedTermHandler CreateHandler()
        {
            return new CreateRelatedTermHandler(
                repository: _mockRepositoryWrapper.Object,
                mapper: _mockMapper.Object,
                logger: _mockLogger.Object);
        }

        private void MockRepositorySetupNullOrEmptyArrOffIds()
        {
            _mockRepositoryWrapper.Setup(x => x.RelatedTermRepository
                    .GetAllAsync(
                        It.IsAny<Expression<Func<Entity, bool>>>(),
                        It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>())) !
                .ReturnsAsync((IEnumerable<Entity>?)null);
        }

        private void SetupMockForExistingTerm(RelatedTermDTO request)
        {
            _mockMapper.Setup(m => m.Map<Entity>(It.IsAny<RelatedTermDTO>())).Returns(new Entity());

            _mockRepositoryWrapper.Setup(x => x.RelatedTermRepository
                    .GetAllAsync(
                        It.Is<Expression<Func<Entity, bool>>>(predicate =>
                            predicate.Compile().Invoke(new Entity { TermId = request.TermId, Word = request.Word })),
                        It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
                .ReturnsAsync(new List<Entity>
                {
                    new Entity()
                    {
                        Id = request.Id,
                        TermId = request.TermId,
                        Word = request.Word,
                    },
                });
        }

        private void SetupMockForSaveChangesFail(RelatedTermDTO request)
        {
            _mockMapper.Setup(m => m.Map<Entity>(It.IsAny<RelatedTermDTO>())).Returns(new Entity());

            _mockRepositoryWrapper.Setup(r => r.RelatedTermRepository
                    .GetAllAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
                .ReturnsAsync(new List<Entity>());

            _mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.Create(It.IsAny<Entity>()));

            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);
        }

        private void SetupMockForMappingFail(RelatedTermDTO request)
        {
            _mockMapper.Setup(m => m.Map<Entity>(It.IsAny<RelatedTermDTO>())).Returns(new Entity());

            _mockRepositoryWrapper.Setup(r => r.RelatedTermRepository
                    .GetAllAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
                .ReturnsAsync(new List<Entity>());

            _mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.Create(It.IsAny<Entity>()));

            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            _mockMapper.Setup(m => m.Map<RelatedTermDTO>(It.IsAny<Entity>())).Returns((RelatedTermDTO)null!);
        }

        private void SetupMockForSuccess(RelatedTermDTO request)
        {
            var relatedTermEntity = new Entity
            {
                Id = request.Id,
                TermId = request.TermId,
                Word = request.Word,
            };

            _mockMapper.Setup(m => m.Map<Entity>(It.IsAny<RelatedTermDTO>())).Returns(relatedTermEntity);

            _mockRepositoryWrapper.Setup(r => r.RelatedTermRepository
                    .GetAllAsync(It.IsAny<Expression<Func<Entity, bool>>>(), It.IsAny<Func<IQueryable<Entity>, IIncludableQueryable<Entity, object>>>()))
                .ReturnsAsync(new List<Entity>());

            _mockRepositoryWrapper.Setup(r => r.RelatedTermRepository.Create(It.IsAny<Entity>()));

            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            _mockMapper.Setup(m => m.Map<RelatedTermDTO>(It.IsAny<Entity>())).Returns(request);
        }

        private RelatedTermDTO GetValidCreateRelatedTermRequest()
        {
            return new RelatedTermDTO
            {
                Id = 1,
                TermId = 1,
                Word = "Test",
            };
        }
    }
