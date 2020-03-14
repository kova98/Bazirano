using Bazirano.Models.DataAccess;
using Bazirano.Models.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bazirano.Infrastructure
{
    public static class SeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Pass123.";
        private const string adminsRole = "Admins";

        public static void EnsureCreated(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            AppIdentityDbContext idContext = app.ApplicationServices.GetRequiredService<AppIdentityDbContext>();
            idContext.Database.Migrate();
        }
        
        public static async void EnsureAdminCreated(IApplicationBuilder app)
        {
            var userManager = app.ApplicationServices.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>();

            bool roleExists = await roleManager.RoleExistsAsync(adminsRole);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(adminsRole));
            }

            IdentityUser user = await userManager.FindByIdAsync(adminUser);
            if (user == null)
            {
                user = new IdentityUser(adminUser);
                await userManager.CreateAsync(user, adminPassword);
                await userManager.AddToRoleAsync(user, adminsRole);
            }
        }
    }
}
