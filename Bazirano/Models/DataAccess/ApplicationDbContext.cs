﻿using Bazirano.Models.Board;
using Bazirano.Models.News;
using Bazirano.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace Bazirano.Models.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<BoardPost> BoardPosts { get; set; }
        public DbSet<BoardThread> BoardThreads { get; set; }
        public DbSet<NewsPost> NewsPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
