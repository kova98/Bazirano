using Bazirano.Models.Board;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Bazirano.Models.DataAccess
{
    public class EFRepository : IBoardThreadsRepository, IBoardPostsRepository
    {
        private ApplicationDbContext context;

        public EFRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<BoardThread> BoardThreads =>
            context.BoardThreads.Include(x => x.Posts);

        public IQueryable<BoardPost> BoardPosts => context.BoardPosts;

        public void AddPost(BoardPost post)
        {
            context.BoardPosts.Add(post);

            context.SaveChanges();
        }

        public void AddPostToThread(BoardPost boardPost, long threadId)
        {
            boardPost.DatePosted = DateTime.Now;

            BoardThread thread = context.BoardThreads
                .Include(x => x.Posts)
                .FirstOrDefault(x => x.Id == threadId);

            if (!string.IsNullOrEmpty(boardPost.Image))
            {
                thread.ImageCount++;
            }

            thread.PostCount++;
            thread?.Posts.Add(boardPost);

            context.SaveChanges();
        }

        public void AddThread(BoardThread thread)
        {
            context.BoardThreads.Add(thread);

            context.SaveChanges();
        }

        public void RemovePost(BoardPost post)
        {
            throw new NotImplementedException();
        }

        public void RemoveThread(BoardThread thread)
        {
            throw new NotImplementedException();
        }
    }
}
