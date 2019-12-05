using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bazirano.Models.DataAccess
{
    /// <summary>
    /// The <see cref="IdentityDbContext"/> used for access to the identity database.
    /// </summary>
    /// <param name="options"></param>
    public class AppIdentityDbContext : IdentityDbContext<IdentityUser>
    { 
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options) { }
    }
}