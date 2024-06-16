namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetById;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

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
        var tag = new Tag();
        var query = new GetTagByIdQuery(1);
        repositoryMock.Setup(repo => repo.TagRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Tag, bool>>>(), default)).ReturnsAsync(tag);

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

        repositoryMock.Setup(repo => repo.TagRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Tag, bool>>>(), default)).ReturnsAsync((Tag)null!);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }
}