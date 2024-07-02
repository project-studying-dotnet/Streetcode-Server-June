using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Persistence;

public static class CustomSeeding
{
    public static async Task SeedFactsAsync(StreetcodeDbContext context)
    {

        // Clear existing data
        context.Facts.RemoveRange(context.Facts);
        await context.SaveChangesAsync();

        // Seed data
        context.Facts.Add(new Streetcode.DAL.Entities.Streetcode.TextContent.Fact { Id = 2 });
        context.Facts.Add(new Streetcode.DAL.Entities.Streetcode.TextContent.Fact { Id = 1 });
        context.Facts.Add(new Streetcode.DAL.Entities.Streetcode.TextContent.Fact { Id = 3 });
        context.Facts.Add(new Streetcode.DAL.Entities.Streetcode.TextContent.Fact { Id = 4 });
        context.Facts.Add(new Streetcode.DAL.Entities.Streetcode.TextContent.Fact { Id = 6 });

        context.Terms.Add(new Streetcode.DAL.Entities.Streetcode.TextContent.Term { Id = 1 });
        context.Terms.Add(new Streetcode.DAL.Entities.Streetcode.TextContent.Term { Id = 2 });


        await context.SaveChangesAsync();
    }

    public static async Task SeedIdentityDataAsync(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        // Constants for roles
        const string adminRoleId = "563b4777-0615-4c3c-8a7d-8858412b6562";

        // Constants for Admin
        const string adminUserName = "SuperAdmin";
        const string adminId = "4eb10d27-a950-45ef-9ebe-f730a07ce5e9";
        const string adminPass = "*Superuser18";

        // Seed Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>("Admin")
            {
                Id = Guid.Parse(adminRoleId)
            });
        }

        // Seed Admin User
        if (await userManager.FindByNameAsync(adminUserName) == null)
        {
            var adminUser = new User()
            {
                UserName = adminUserName,
                Id = Guid.Parse(adminId),
                Email = "admin@example.com",
            };

            var result = await userManager.CreateAsync(adminUser, adminPass);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}