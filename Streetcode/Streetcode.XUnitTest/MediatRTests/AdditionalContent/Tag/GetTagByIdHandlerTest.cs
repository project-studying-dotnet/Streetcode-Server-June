using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;

using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag;

public class GetTagByIdHandlerTest
{
    private readonly Mock<IRepositoryWrapper> repositoryMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<ILoggerService> loggerMock;
    private readonly GetTagByIdHandler handler;

    public GetTagByIdHandlerTest()
    {
        repositoryMock = new Mock<IRepositoryWrapper>();
        mapperMock = new Mock<IMapper>();
        loggerMock = new Mock<ILoggerService>();
        handler = new GetTagByIdHandler(repositoryMock.Object, mapperMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenTagExists()
    {
        // Arrange
        var tag = new DAL.Entities.AdditionalContent.Tag();
        var query = new GetTagByIdQuery(1);
        repositoryMock.Setup(repo => repo.TagRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<DAL.Entities.AdditionalContent.Tag, bool>>>(), default)).ReturnsAsync(tag);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Test_Handle_ArtDoesNotExist_ReturnsErrorResult_IsSuccessIsFalse()
    {
        // Arrange
        var request = new GetTagByIdQuery(1);

        repositoryMock.Setup(repo => repo.TagRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<DAL.Entities.AdditionalContent.Tag, bool>>>(), default)).ReturnsAsync((DAL.Entities.AdditionalContent.Tag)null!);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }
}