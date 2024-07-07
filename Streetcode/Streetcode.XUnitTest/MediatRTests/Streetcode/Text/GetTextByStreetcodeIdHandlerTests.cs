namespace Streetcode.XUnitTest.MediatRTests.StreetcodeTests.Text;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Entities.Streetcode;

public class GetTextByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> mockRepo;
    private readonly Mock<IMapper> mockMapper;
    private readonly Mock<ITextService> mockTextService;
    private readonly Mock<ILoggerService> mockLogger;

    public GetTextByStreetcodeIdHandlerTests()
    {
        mockRepo = new Mock<IRepositoryWrapper>();
        mockMapper = new Mock<IMapper>();
        mockTextService = new Mock<ITextService>();
        mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_Should_ReturnsTexts_WhenTextsExist()
    {
        // Arrange
        var handler = CreateHandler(GetTextList(), GetTextDtoList(), 1, streetcodeExists: true);

        // Act
        var result = await handler.Handle(new GetTextByStreetcodeIdQuery(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(GetTextDtoList().Count(), result.Value.Count());
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyResult_WhenNoTextsExist()
    {
        // Arrange
        var handler = CreateHandler(new List<Text>(), new List<TextDTO>(), 1, streetcodeExists: true);

        // Act
        var result = await handler.Handle(new GetTextByStreetcodeIdQuery(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailResult_WhenStreetcodeDoesNotExist()
    {
        // Arrange
        var handler = CreateHandler(new List<Text>(), new List<TextDTO>(), 1, streetcodeExists: false);

        // Act
        var result = await handler.Handle(new GetTextByStreetcodeIdQuery(1), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal($"No streetcodes exist now", result.Errors.First().Message);
        mockLogger.Verify(logger => logger.LogError(It.IsAny<GetTextByStreetcodeIdQuery>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnTexts_WithTermsTags()
    {
        // Arrange
        var handler = CreateHandler(GetTextList(), GetTextDtoListWithTaggedContent(), 1, streetcodeExists: true, tagContent: true);

        // Act
        var result = await handler.Handle(new GetTextByStreetcodeIdQuery(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(GetTextDtoListWithTaggedContent().Count(), result.Value.Count());
        Assert.All(result.Value, dto => Assert.StartsWith("tagged_", dto.TextContent));
    }

    private GetTextByStreetcodeIdHandler CreateHandler(IEnumerable<Text> textList, IEnumerable<TextDTO> textDtoList, int streetcodeId, bool streetcodeExists = true, bool tagContent = false)
    {
        MockRepository(textList, streetcodeId, streetcodeExists);
        MockMapper(textDtoList);
        MockTextService(tagContent);

        return new GetTextByStreetcodeIdHandler(mockRepo.Object, mockMapper.Object, mockTextService.Object, mockLogger.Object);
    }

    private static List<Text> GetTextList() => new()
    {
        new Text { Id = 1, Title = "Title 1", TextContent = "Content 1", StreetcodeId = 1 },
        new Text { Id = 2, Title = "Title 2", TextContent = "Content 2", StreetcodeId = 1 }
    };

    private static List<TextDTO> GetTextDtoList() => new()
    {
        new TextDTO { Id = 1, Title = "Title 1", TextContent = "Content 1", StreetcodeId = 1 },
        new TextDTO { Id = 2, Title = "Title 2", TextContent = "Content 2", StreetcodeId = 1 }
    };

    private static List<TextDTO> GetTextDtoListWithTaggedContent() => new()
    {
        new TextDTO { Id = 1, Title = "Title 1", TextContent = "tagged_Content 1", StreetcodeId = 1 },
        new TextDTO { Id = 2, Title = "Title 2", TextContent = "tagged_Content 2", StreetcodeId = 1 }
    };

    private void MockRepository(IEnumerable<Text> textList, int streetcodeId, bool streetcodeExists)
    {
        mockRepo.Setup(repo => repo.TextRepository.GetAllAsync(
                It.IsAny<Expression<Func<Text, bool>>>(),
                It.IsAny<Func<IQueryable<Text>, IIncludableQueryable<Text, object>>>()))
            .ReturnsAsync(textList.ToList());

        if (streetcodeExists)
        {
            mockRepo.Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync(new StreetcodeContent { Id = streetcodeId });
        }
        else
        {
            mockRepo.Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync((StreetcodeContent)null!);
        }
    }

    private void MockMapper(IEnumerable<TextDTO> textDtoList)
    {
        mockMapper.Setup(mapper => mapper.Map<IEnumerable<TextDTO>>(It.IsAny<IEnumerable<Text>>())).Returns(textDtoList);
    }

    private void MockTextService(bool tagContent)
    {
        if (tagContent)
        {
            mockTextService.Setup(service => service.AddTermsTag(It.IsAny<string>()))
                .ReturnsAsync((string textContent) => $"tagged_{textContent}");
        }
        else
        {
            mockTextService.Setup(service => service.AddTermsTag(It.IsAny<string>()))
                .ReturnsAsync((string textContent) => textContent);
        }
    }
}
