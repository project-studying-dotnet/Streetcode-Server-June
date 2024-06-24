using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.XUnitTest.MediatRTests.Media.Art
{
    public class DeleteArtHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private DeleteArtHandler _handler;

        public DeleteArtHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenArtFound()
        {
            // Arrange
            MockRepositoryWrapperSetupWithExistingArtId(1);

            _handler = new DeleteArtHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await _handler.Handle(new DeleteArtCommand(1), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_Should_LogCorrectErrorMessage_WhenArtIsNotFound()
        {
            // Arrange
            MockRepositoryWrapperSetupWithNotExistingArtId();

            _handler = new DeleteArtHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            var expectedErrorMessage = MessageResourceContext.GetMessage(ErrorMessages.EntityWithIdNotFound, new DeleteArtCommand(1), 1);

            // Act
            var result = await _handler.Handle(new DeleteArtCommand(1), CancellationToken.None);
            var actualErrorMessage = result.Errors[0].Message;

            // Assert
            Assert.Equal(expectedErrorMessage, actualErrorMessage);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenSaveChangesAsyncNotSuccessful()
        {
            // Arrange
            _repositoryWrapperMock.Setup(x => x.ArtRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<ArtEntity>,
                    IIncludableQueryable<ArtEntity, object>>>()))
                .ReturnsAsync(new ArtEntity { Id = 1 });

            _repositoryWrapperMock.Setup(x => x.ArtRepository
                .Delete(new ArtEntity { Id = 1 }));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).Throws(new InvalidOperationException("Save failed"));

            _handler = new DeleteArtHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);

            // Act
            var result = await _handler.Handle(new DeleteArtCommand(1), CancellationToken.None);

            // Assert
            Assert.True(result.Errors[0].Message == "Save failed");
        }

        private void MockRepositoryWrapperSetupWithExistingArtId(int id)
        {
            _repositoryWrapperMock.Setup(x => x.ArtRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<ArtEntity>,
                    IIncludableQueryable<ArtEntity, object>>>()))
                .ReturnsAsync(new ArtEntity { Id = 1 });

            _repositoryWrapperMock.Setup(x => x.ArtRepository
                .Delete(new ArtEntity { Id = 1 }));

            _repositoryWrapperMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        }

        private void MockRepositoryWrapperSetupWithNotExistingArtId(bool returnNull = true)
        {
            _repositoryWrapperMock.Setup(x => x.ArtRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<ArtEntity>,
                    IIncludableQueryable<ArtEntity, object>>>()))
                .ReturnsAsync((ArtEntity)null!);
        }
    }
}
