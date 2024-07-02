using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Entities.Users;
using Xunit;

public class AdminPolicyTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AdminPolicyTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
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
        var token = await tokenService.GenerateAccessToken(adminUser, claims);
        return token;
    }

    private void AddAuthorizationHeader(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task Create_Position_AuthorizedAsAdmin_ReturnsOk()
    {
        // Arrange
        var token = await GetAdminToken();
        AddAuthorizationHeader(_client, token);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Position/Create")
        {
            Content = new StringContent("{\"Position\": \"New Fact\"}", Encoding.UTF8, "application/json")

        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Create_Position_Unauthorized_ReturnsForbidden()
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
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Fact_AuthorizedAsAdmin_ReturnsOk()
    {
        // Arrange
        var token = await GetAdminToken(); // simulate admin user
        AddAuthorizationHeader(_client, token);
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/Fact/Delete/1");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Fact_Unauthorized_ReturnsForbidden()
    {
        // Arrange
        var token = "SomeInvalidToken"; // Simulate non-admin user
        AddAuthorizationHeader(_client, token);
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/Fact/Delete/1");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_Term_AuthorizedAsAdmin_ReturnsOk()
    {
        // Arrange
        var token = await GetAdminToken();
        AddAuthorizationHeader(_client, token);
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/Term/Update")
        {
            Content = new StringContent("{\"Id\": \"1\", \"title\": \"value\", \"description\": \"value\"}", Encoding.UTF8, "application/json")
        };

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_Term_Unauthorized_ReturnsForbidden()
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
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
