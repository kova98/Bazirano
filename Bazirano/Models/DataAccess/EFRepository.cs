using Bazirano.Infrastructure;
using Bazirano.Models.Board;
using Bazirano.Models.Column;
using Bazirano.Models.News;
using Bazirano.Models.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.DataAccess
{

    public class EFRepository : IBoardThreadsRepository, IBoardPostsRepository, INewsPostsRepository, IColumnRepository
    {
        private ApplicationDbContext context;
        private IWriter writer;

        public EFRepository(ApplicationDbContext ctx, IWriter writer)
        {
            context = ctx;
            this.writer = writer;
        }

        public IQueryable<BoardThread> BoardThreads => context.BoardThreads
            .Include(x => x.Posts);

        public IQueryable<BoardPost> BoardPosts => context.BoardPosts;

        public IQueryable<NewsPost> NewsPosts => context.NewsPosts
            .Include(x=>x.Comments);

        public IQueryable<ColumnPost> ColumnPosts => context.ColumnPosts
            .Include(p => p.Author)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Responses);

        public IQueryable<Author> Authors => context.Authors;

        public void AddCommentResponse(Comment responseComment, long commentId)
        {
            Comment comment = context.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment.Responses == null)
            {
                comment.Responses = new List<Comment>();
            }

            responseComment.IsRoot = false;
            comment.Responses.Add(responseComment);

            context.SaveChanges();
        }

        public void AddCommentToNewsPost(Comment comment, long postId)
        {
            NewsPost post = context.NewsPosts.FirstOrDefault(p => p.Id == postId);
            if (post.Comments == null)
            {
                post.Comments = new List<Comment>();
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
            NewsPost postToEdit = context.NewsPosts.FirstOrDefault(x => x.Id == post.Id);

            postToEdit.Title = post.Title;
            postToEdit.Text = post.Text;
            postToEdit.Image = post.Image;

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

        public void RemoveNewsPost(long postId)
        {
            var post = context.NewsPosts.FirstOrDefault(p => p.Id == postId);

            if (post.Comments != null && post.Comments.Count > 0)
            {
                var comments = context.Comments.Where(x => post.Comments.Contains(x));

                context.Comments.RemoveRange(comments);
            }
            
            context.NewsPosts.Remove(post);

            context.SaveChanges();
        }

        public void RemovePost(BoardPost post)
        {
            throw new NotImplementedException();
        }

        public void RemoveThread(BoardThread thread)
        {
            var posts = context.BoardPosts.Where(x => thread.Posts.Contains(x));

            foreach (var post in posts)
            {
                writer.DeleteImage(post.Image);
            }

            context.BoardPosts.RemoveRange(posts);

            context.BoardThreads.Remove(thread);

            context.SaveChanges();
        }

        public void IncrementViewCount(NewsPost post)
        {
            context.NewsPosts.FirstOrDefault(x=>x.Id == post.Id).ViewCount += 1;

            context.SaveChanges();
        }

        public void AddColumn(ColumnPost column)
        {
            context.ColumnPosts.Add(column);

            context.SaveChanges();
        }

        public void SaveColumn(ColumnPost column)
        {
            var existing = context.ColumnPosts
                .FirstOrDefault(p => p.Id == column.Id);

            if (existing != null)
            {
                existing.Author = context.Authors.First(a => a.Id == column.Author.Id);
                existing.Text = column.Text;
                existing.Title = column.Title;
                existing.Image = column.Image;
            } 
            else
            {
                column.DatePosted = DateTime.Now;
                column.Author = context.Authors.First(a => a.Id == column.Author.Id);
                context.ColumnPosts.Add(column);
            }

            context.SaveChanges();
        }

        public void DeleteColumn(long columnId)
        {
            var column = context.ColumnPosts.FirstOrDefault(c => c.Id == columnId);

            context.ColumnPosts.Remove(column);

            context.SaveChanges();
        }

        public void SaveAuthor(Author author)
        {
            var existing = context.Authors.FirstOrDefault(a => a.Id == author.Id);

            if (existing != null)
            {
                existing.Name = author.Name;
                existing.ShortBio = author.ShortBio;
                existing.Bio = author.Bio;
                existing.Image = author.Image;
            }
            else
            {
                context.Authors.Add(author);
            }

            context.SaveChanges();
        }

        public void AddCommentToColumn(Comment comment, long columnId)
        {
            ColumnPost post = context.ColumnPosts.FirstOrDefault(p => p.Id == columnId);
            if (post.Comments == null)
            {
                post.Comments = new List<Comment>();
            }

            post.Comments.Add(comment);

            context.SaveChanges();
        }

        public void DeleteAuthor(long authorId)
        {
            var author = context.Authors.FirstOrDefault(a => a.Id == authorId);

            context.Authors.Remove(author);

            context.SaveChanges();
        }
    }
}
