using Microsoft.Extensions.Options;
using Moq.Protected;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Services.Instagram;
using Xunit;

namespace Streetcode.XUnitTest.ServicesTests;

public class InstagramServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly HttpClient _httpClient;
    private readonly IOptions<InstagramEnvirovmentVariables> _options;
    private readonly string _responseContent;

    public InstagramServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _loggerMock = new Mock<ILoggerService>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(x => x.CreateClient("InstagramClient")).Returns(_httpClient);
        _responseContent = "Sorry, this content isn't available right now";

        var settings = new InstagramEnvirovmentVariables
        {
            MediaRequestUrl = "https://graph.instagram.com/test",
            InstagramID = "test_id",
            InstagramToken = "test_token"
        };
        _options = Options.Create(settings);
    }

    [Fact]
    public async Task GetPostsAsync_ShouldReturnPosts_WhenResponseIsSuccessful()
    {
        // Arrange
        var responseContent = "{\"data\":[{\"id\":\"1\",\"media_type\":\"IMAGE\",\"media_url\":\"url1\",\"caption\":\"caption1\"}]}";
        var responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        var service = new InstagramService(_httpClientFactoryMock.Object, _options, _loggerMock.Object);

        // Act
        var posts = await service.GetPostsAsync();

        // Assert
        Assert.Multiple(
            () => Assert.Single(posts),
            () => Assert.Equal("1", posts.First().Id),
            () => Assert.Equal("caption1", posts.First().Caption));
    }

    [Fact]
    public async Task GetPostsAsync_ShouldThrowHttpRequestException_WhenResponseIsBadRequest()
    {
        // Arrange
        var exceptionMsg = $"Status code 400 from Instagram API: {_responseContent}";
        var responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
        {
            Content = new StringContent(_responseContent)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        var service = new InstagramService(_httpClientFactoryMock.Object, _options, _loggerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => service.GetPostsAsync());
        Assert.Contains(exceptionMsg, exception.Message);
    }

    [Fact]
    public async Task GetPostsAsync_ShouldThrowHttpRequestException_WhenResponseIsForbidden()
    {
        // Arrange
        var exceptionMsg = $"Status code 403 from Instagram API: {_responseContent}";
        var responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
        {
            Content = new StringContent(_responseContent)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        var service = new InstagramService(_httpClientFactoryMock.Object, _options, _loggerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => service.GetPostsAsync());
        Assert.Contains(exceptionMsg, exception.Message);
    }
}
