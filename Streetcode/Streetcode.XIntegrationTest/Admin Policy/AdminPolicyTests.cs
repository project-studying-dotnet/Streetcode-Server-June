using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Streetcode.DAL.Entities.Users;
using Xunit;

public class AdminPolicyTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AdminPolicyTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public void Dispose()
    {
       // _client.Dispose();
       // _factory.Dispose();
    }

    private async Task<string> GetAdminToken()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var token = await LoginAdminUserAsync(userManager);
        return token;
    }

    private async Task<string> LoginAdminUserAsync(UserManager<User> userManager)
    {
        const string adminUserName = "SuperAdmin";
        const string adminPass = "*Superuser18";

        var adminUser = await userManager.FindByNameAsync(adminUserName);
        if (adminUser == null)
        {
            throw new Exception("Admin user not found.");
        }

        var token = await GenerateJwtToken(adminUser, userManager);
        return token;
    }

    private async Task<string> GenerateJwtToken(User user, UserManager<User> userManager)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(30);

        var token = new JwtSecurityToken(
            issuer: "yourIssuer",
            audience: "yourAudience",
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
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
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response: {responseContent}"); // Log the response content

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
        var token = await GetAdminToken();
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
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response: {responseContent}"); // Log the response content

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
