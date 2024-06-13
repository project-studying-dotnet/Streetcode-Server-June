namespace Streetcode.XUnitTest.MediatRTests.Media.Art;
using AutoMapper;
using Moq;
using Streetcode.BLL.Enums;
using Streetcode.BLL.Exceptions;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Linq.Expressions;
using System.Net;
using Xunit;
public class GetArtByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<ILoggerService> loggerMock;
    private readonly GetArtByIdHandler handler;

    public GetArtByIdHandlerTests()
    {
        repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        mapperMock = new Mock<IMapper>();
        loggerMock = new Mock<ILoggerService>();
        handler = new GetArtByIdHandler(repositoryWrapperMock.Object, mapperMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenArtExists()
    {
        // Arrange
        var artId = 1;
        var request = new GetArtByIdQuery(artId);
        var artEntity = new DAL.Entities.Media.Images.Art { Id = artId };

        repositoryWrapperMock.Setup(repo => repo.ArtRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.Media.Images.Art, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.Media.Images.Art>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<DAL.Entities.Media.Images.Art, object>>>()))
            .ReturnsAsync(artEntity);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenAriWithIdNotExists()
    {
        // Arrange
        var artId = 1;
        var request = new GetArtByIdQuery(artId);

        repositoryWrapperMock.Setup(repo => repo.ArtRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.Media.Images.Art, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.Media.Images.Art>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<DAL.Entities.Media.Images.Art, object>>>()))
            .ReturnsAsync(null as DAL.Entities.Media.Images.Art);

        // Act
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(() => handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal("Entity 'Art' with id '1' not found", result.Message);
    }
}