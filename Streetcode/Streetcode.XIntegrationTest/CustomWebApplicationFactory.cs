using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.Extensions;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> 
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            // Remove the app's StreetcodeDbContext registration.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<StreetcodeDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Remove the AddApplicationServices registration
            var applicationServiceDescriptor = services.SingleOrDefault(
                d => d.ImplementationFactory != null && d.ImplementationFactory.Method.Name.Contains("AddApplicationServices"));
            if (applicationServiceDescriptor != null)
            {
                services.Remove(applicationServiceDescriptor);
            }

            // Add an in-memory database context.
            services.AddDbContext<StreetcodeDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Apply the access token configuration.
            var authenticationDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>));
            if (authenticationDescriptor == null)
            {
                services.AddTokensConfiguration(context.Configuration);
            }

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
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();
                    logger.LogError(ex, "An error occurred seeding the database.");
                }
            }

            services.Configure<HostOptions>(hostOptions =>
            {
                hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });
        });
        builder.UseEnvironment("IntegrationTests");

    }
}
