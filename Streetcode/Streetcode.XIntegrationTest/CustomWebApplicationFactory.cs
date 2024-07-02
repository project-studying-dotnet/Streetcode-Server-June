using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Persistence;
using System.Text;
using System.Threading.Tasks;

public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    private BackgroundJobServer _hangfireServer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's StreetcodeDbContext registration.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<StreetcodeDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add an in-memory database context.
            services.AddDbContext<StreetcodeDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database contexts.
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StreetcodeDbContext>();
                var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                // Seed the database with test data.
                try
                {
                    CustomSeeding.SeedFactsAsync(db).Wait();
                    CustomSeeding.SeedIdentityDataAsync(userManager, roleManager).Wait();
                }
                catch (Exception ex)
                {
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TEntryPoint>>>();
                    logger.LogError(ex, "An error occurred seeding the database.");
                }
            }

            // Remove any existing authentication schemes
            var authenticationDescriptors = services.Where(d => d.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>)).ToList();
            foreach (var descriptorr in authenticationDescriptors)
            {
                services.Remove(descriptorr);
            }

            // Add JWT Bearer Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "yourIssuer",
                    ValidAudience = "yourAudience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey"))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            });

            services.AddScoped<AdminPolicyAttribute>();

            // Add Hangfire configuration conditionally for tests
            services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "default" };
                options.ServerName = "InMemoryHangfire";
            });
        });

        //builder.Configure(app =>
        //{
        //    // Ensure proper middleware order
        //    app.UseRouting();
        //    app.UseAuthentication();
        //    app.UseAuthorization();

        //    // Add Hangfire Dashboard after routing and authentication
        //    app.UseHangfireDashboard();

        //    // Start Hangfire server
        //    var backgroundJobServerOptions = new BackgroundJobServerOptions
        //    {
        //        ServerName = "InMemoryHangfire",
        //        Queues = new[] { "default" }
        //    };

        //    _hangfireServer = new BackgroundJobServer(backgroundJobServerOptions);

        //    // Map controllers after configuring Hangfire
        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllers();
        //    });
        //});
    }

    public override async ValueTask DisposeAsync()
    {
        //_hangfireServer.SendStop();
        //await _hangfireServer.WaitForShutdownAsync(CancellationToken.None);

        //await base.DisposeAsync();
    }
}
