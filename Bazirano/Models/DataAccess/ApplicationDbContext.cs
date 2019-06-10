using Bazirano.Models.Board;
using Microsoft.EntityFrameworkCore;

namespace Bazirano.Models.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<BoardPost> BoardPosts { get; set; }
        public DbSet<BoardThread> BoardThreads { get; set; }
    }
}
