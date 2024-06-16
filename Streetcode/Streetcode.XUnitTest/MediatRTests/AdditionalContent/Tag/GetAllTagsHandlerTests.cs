using AutoMapper;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag;

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
        repositoryMock.Setup(repo => repo.TagRepository.GetAllAsync(default, default)).ReturnsAsync((IEnumerable<DAL.Entities.AdditionalContent.Tag>)null!);

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
        IEnumerable<DAL.Entities.AdditionalContent.Tag> tags = new List<DAL.Entities.AdditionalContent.Tag>() { new DAL.Entities.AdditionalContent.Tag() };
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
        IEnumerable<DAL.Entities.AdditionalContent.Tag> tags = new List<DAL.Entities.AdditionalContent.Tag>() { new DAL.Entities.AdditionalContent.Tag() };
        IEnumerable<TagDTO> tagsDTO = new List<TagDTO>() { new TagDTO() };
        repositoryMock.Setup(repo => repo.TagRepository.GetAllAsync(default, default)).ReturnsAsync(tags);
        mapperMock.Setup(mapper => mapper.Map<IEnumerable<TagDTO>>(It.IsAny<IEnumerable<DAL.Entities.AdditionalContent.Tag>>())).Returns(tagsDTO);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(tagsDTO, result.Value);
    }
}