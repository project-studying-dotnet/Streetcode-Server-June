namespace Streetcode.XUnitTest.MediatRTests.Media.Art;
using AutoMapper;
using BLL.Exceptions;
using BLL.Interfaces.Logging;
using Moq;
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
        MockRepository(artEntity);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenArtWithIdNotExists()
    {
        // Arrange
        var artId = 1;
        var request = new GetArtByIdQuery(artId);
        MockRepository(null);

        // Act
        var result = await Assert.ThrowsAsync<RequestException>(() => handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal($"Cannot find an art with corresponding id: {request.Id}", result.Message);
    }

    private void MockRepository(DAL.Entities.Media.Images.Art? artEntity)
    {
        repositoryWrapperMock.Setup(repo => repo.ArtRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.Media.Images.Art, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.Media.Images.Art>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<DAL.Entities.Media.Images.Art, object>>>()))
            .ReturnsAsync(artEntity);
    }
}