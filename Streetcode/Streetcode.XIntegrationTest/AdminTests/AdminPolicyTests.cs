using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Entities.Users;
using Xunit;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

public class AdminPolicyTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private HttpClient _client;
    private CustomWebApplicationFactory<Program> _factory;

    public AdminPolicyTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_Position_AuthorizedAsAdmin_ReturnsOk()
    {
        // Arrange
        var token = await GetAdminToken();
        AddAuthorizationHeader(_client, token);
        var content = new StringContent("{\"position\":\"New Fact\"}", Encoding.UTF8, "application/json");
        var contentString = await content.ReadAsStringAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Position/Create")
        {
            Content = content
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        // Act
        var response = await _client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        var positionResponse = JsonSerializer.Deserialize<PositionDTO>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(positionResponse.Position, contentString);
        });
    }

    [Fact]
    public async Task Create_Position_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var token = "SomeInvalidToken"; // Simulate non-admin user
        AddAuthorizationHeader(_client, token);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Position/Create")
        {
            Content = new StringContent("{\"Position\": \"New Fact\"}", Encoding.UTF8, "application/json")
        };

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Fact_AuthorizedAsAdmin_ReturnsOk()
    {
        // Arrange
        var token = await GetAdminToken(); // simulate admin user
        AddAuthorizationHeader(_client, token);
        var factId = 1;
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/Fact/Delete/{factId}");

        // Act
        var response = await _client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        var positionResponse = JsonSerializer.Deserialize<FactDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(positionResponse.Id, factId);
        });
    }

    [Fact]
    public async Task Delete_Fact_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var token = "SomeInvalidToken"; // Simulate non-admin user
        AddAuthorizationHeader(_client, token);
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/Fact/Delete/1");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_Term_AuthorizedAsAdmin_ReturnsOk()
    {
        // Arrange
        var token = await GetAdminToken();
        AddAuthorizationHeader(_client, token);
        var content = new StringContent("{\"id\":1,\"title\":\"value\",\"description\":\"value\"}", Encoding.UTF8, "application/json");
        var contentString = content.ReadAsStringAsync();
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/Term/Update")
        {
            Content = content
        };

        // Act
        var response = await _client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(contentString.Result, responseContent);
        });
    }

    [Fact]
    public async Task Update_Term_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var token = "SomeInvalidToken"; // Simulate non-admin user
        AddAuthorizationHeader(_client, token);
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/Term/Update")
        {
            Content = new StringContent("{\"title\": \"value\"}", Encoding.UTF8, "application/json")
        };

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    private async Task<string> GetAdminToken()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var token = await LoginAdminUserAsync(userManager, tokenService);
        return token;
    }

    private async Task<string> LoginAdminUserAsync(UserManager<User> userManager, ITokenService tokenService)
    {
        const string adminUserName = "SuperAdmin";

        var adminUser = await userManager.FindByNameAsync(adminUserName);
        if (adminUser == null)
        {
            throw new Exception("Admin user not found.");
        }

        var claims = await tokenService.GetUserClaimsAsync(adminUser);
        var token = tokenService.GenerateAccessToken(adminUser, claims);
        return token;
    }

    private void AddAuthorizationHeader(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
