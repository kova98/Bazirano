using Bazirano.Infrastructure;
using Bazirano.Models.AuthorInterface;
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

    public class EFRepository : IBoardThreadRepository, IBoardPostRepository, IArticleRepository, IColumnRepository, IColumnRequestRepository
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

        public IQueryable<Article> Articles => context.Articles
            .Include(a => a.Discussion)
                .ThenInclude(d => d.Posts);

        public IQueryable<ColumnPost> ColumnPosts => context.ColumnPosts
            .Include(p => p.Author)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Responses);

        public IQueryable<Author> Authors => context.Authors;

        public IQueryable<ColumnRequest> ColumnRequests => context.ColumnRequests
            .Include(c => c.Author);

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

        public void AddArticle(Article post)
        {
            context.Articles.Add(post);

            context.SaveChanges();
        }

        public void EditArticle(Article post)
        {
            Article postToEdit = context.Articles.FirstOrDefault(x => x.Id == post.Id);

            postToEdit.Title = post.Title;
            postToEdit.Text = post.Text;
            postToEdit.Image = post.Image;
            postToEdit.Discussion = post.Discussion;

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

        public void RemoveArticle(long postId)
        {
            var post = context.Articles.FirstOrDefault(p => p.Id == postId);

            context.Articles.Remove(post);

            context.SaveChanges();
        }

        public void RemovePost(BoardPost post)
        {
            throw new NotImplementedException();
        }

        public void RemoveThread(long id)
        {
            var thread = context.BoardThreads
                .Include(x=>x.Posts)
                .FirstOrDefault(x => x.Id == id);

            foreach (var post in thread.Posts)
            {
                writer.DeleteImage(post.Image);
            }

            context.BoardPosts.RemoveRange(thread.Posts);

            context.BoardThreads.Remove(thread);

            context.SaveChanges();
        }

        public void UpdateThread(BoardThread thread)
        {
            var oldThread = context.BoardThreads.First(x => x.Id == thread.Id);

            oldThread.SafeForWork = thread.SafeForWork;
            oldThread.ImageCount = thread.ImageCount;
            oldThread.PostCount = thread.PostCount;
            oldThread.IsLocked = thread.IsLocked;

            context.SaveChanges();
        }

        public void IncrementViewCount(Article post)
        {
            context.Articles.FirstOrDefault(x=>x.Id == post.Id).ViewCount += 1;

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

        public void AddColumnRequest(ColumnRequest columnRequest)
        {
            context.ColumnRequests.Add(columnRequest);

            context.SaveChanges();
        }

        public void RemoveColumnRequest(long columnRequestId)
        {
            var columnRequest = context.ColumnRequests.FirstOrDefault(c => c.Id == columnRequestId);

            if (columnRequest == null)
            {
                throw new InvalidOperationException($"Tried to remove a non existing ColumnRequest (columnRequestId = {columnRequestId})");
            }

            context.ColumnRequests.Remove(columnRequest);

            context.SaveChanges();
        }

        public void EditColumnRequest(ColumnRequest columnRequest)
        {
            var existing = context.ColumnRequests.FirstOrDefault(c => c.Id == columnRequest.Id);

            context.Entry(existing).State = EntityState.Detached;
            context.ColumnRequests.Update(columnRequest);

            context.SaveChanges();
        }
    }
}
