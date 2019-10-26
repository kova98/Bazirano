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

        public static void EnsureCreated(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            AppIdentityDbContext idContext = app.ApplicationServices.GetRequiredService<AppIdentityDbContext>();
            idContext.Database.Migrate();
        }
        
        public static async void EnsureAdminCreated(IApplicationBuilder app)
        {
            UserManager<IdentityUser> userManager = app.ApplicationServices
                .GetRequiredService<UserManager<IdentityUser>>();

            IdentityUser user = await userManager.FindByIdAsync(adminUser);
            if (user == null)
            {
                user = new IdentityUser("Admin");
                await userManager.CreateAsync(user, adminPassword);
            }
        }
    }
}
