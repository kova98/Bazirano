using Bazirano.Models.Board;
using Bazirano.Models.News;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bazirano.Models.DataAccess
{
    public class EFRepository : IBoardThreadsRepository, IBoardPostsRepository, INewsPostsRepository
    {
        private ApplicationDbContext context;

        public EFRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<BoardThread> BoardThreads => context.BoardThreads
            .Include(x => x.Posts);

        public IQueryable<BoardPost> BoardPosts => context.BoardPosts;

        public IQueryable<NewsPost> NewsPosts => context.NewsPosts
            .Include(x=>x.Comments);

        public void AddCommentToNewsPost(NewsComment comment, long postId)
        {
            NewsPost post = context.NewsPosts.First(p => p.Id == postId);
            if (post.Comments == null)
            {
                post.Comments = new List<NewsComment>();
            }

            post.Comments.Add(comment);

            context.SaveChanges();
        }

        public void AddNewsPost(NewsPost post)
        {
            context.NewsPosts.Add(post);

            context.SaveChanges();
        }

        public void EditNewsPost(NewsPost post)
        {
            NewsPost postToEdit = context.NewsPosts.First(x => x.Id == post.Id);

            postToEdit.Title = post.Title;
            postToEdit.Text = post.Text;

            context.SaveChanges();
        }

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

        public void RemoveNewsPost(NewsPost post)
        {
            // TODO: Remove the related comments as well

            context.NewsPosts.Remove(post);

            context.SaveChanges();
        }

        public void RemovePost(BoardPost post)
        {
            throw new NotImplementedException();
        }

        public void RemoveThread(BoardThread thread)
        {
            // TODO: Remove the related posts as well

            context.BoardThreads.Remove(thread);

            context.SaveChanges();
        }

        public void IncrementViewCount(NewsPost post)
        {
            NewsPost record = context.NewsPosts.FirstOrDefault(x=>x.Id == post.Id);
            record.ViewCount++;

            context.SaveChanges();
        }
    }
}
