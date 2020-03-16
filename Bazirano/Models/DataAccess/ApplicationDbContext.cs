using Bazirano.Models.AuthorInterface;
using Bazirano.Models.Board;
using Bazirano.Models.Column;
using Bazirano.Models.News;
using Bazirano.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace Bazirano.Models.DataAccess
{
    /// <summary>
    /// The <see cref="DbContext"/> used for access to the application database.
    /// </summary>
    /// <param name="options"></param>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        /// <summary>
        /// The collection of <see cref="BoardPost"/>s in the application database.
        /// </summary>
        public DbSet<BoardPost> BoardPosts { get; set; }

        /// <summary>
        /// The collection of <see cref="BoardThread"/>s in the application database.
        /// </summary>
        public DbSet<BoardThread> BoardThreads { get; set; }

        /// <summary>
        /// The collection of <see cref="NewsPost"/>s in the application database.
        /// </summary>
        public DbSet<NewsPost> NewsPosts { get; set; }

        /// <summary>
        /// The collection of <see cref="Comment"/>s in the application database.
        /// </summary>
        public DbSet<Comment> Comments { get; set; }

        public DbSet<ColumnPost> ColumnPosts { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<ColumnRequest> ColumnRequests { get; set; }
    }
}
