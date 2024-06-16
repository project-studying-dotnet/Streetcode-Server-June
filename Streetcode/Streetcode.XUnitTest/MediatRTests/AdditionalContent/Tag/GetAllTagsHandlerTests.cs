namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetAll;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class GetAllTagsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> repositoryMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<ILoggerService> loggerMock;
    private readonly GetAllTagsHandler handler;

    public GetAllTagsHandlerTests()
    {
        repositoryMock = new Mock<IRepositoryWrapper>();
        mapperMock = new Mock<IMapper>();
        loggerMock = new Mock<ILoggerService>();
        handler = new GetAllTagsHandler(repositoryMock.Object, mapperMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenRepositoryReturnsNull()
    {
        // Arrange
        var request = new GetAllTagsQuery();
        repositoryMock.Setup(repo => repo.TagRepository.GetAllAsync(default, default)).ReturnsAsync((IEnumerable<Tag>)null!);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenRepositoryReturnsData()
    {
        // Arrange
        var request = new GetAllTagsQuery();
        IEnumerable<Tag> tags = new List<Tag>() { new Tag() };
        repositoryMock.Setup(repo => repo.TagRepository.GetAllAsync(default, default)).ReturnsAsync(tags);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_Should_ReturnsMappedTags_WhenRepositoryReturnsData()
    {
        // Arrange
        var request = new GetAllTagsQuery();
        IEnumerable<Tag> tags = new List<Tag>() { new Tag() };
        IEnumerable<TagDTO> tagsDTO = new List<TagDTO>() { new TagDTO() };
        repositoryMock.Setup(repo => repo.TagRepository.GetAllAsync(default, default)).ReturnsAsync(tags);
        mapperMock.Setup(mapper => mapper.Map<IEnumerable<TagDTO>>(It.IsAny<IEnumerable<Tag>>())).Returns(tagsDTO);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(tagsDTO, result.Value);
    }
}