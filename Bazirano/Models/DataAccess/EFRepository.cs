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
    /// <summary>
    /// The Entity Framework repository class used for retrieving data from the application database.
    /// </summary>
    public class EFRepository : IBoardThreadsRepository, IBoardPostsRepository, INewsPostsRepository, IColumnRepository
    {
        private ApplicationDbContext context;

        public EFRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        /// <summary>
        /// The <see cref="IQueryable"/> collection of all the <see cref="BoardThread"/>s in the database, loaded with child <see cref="BoardPost"/>s.
        /// </summary>
        public IQueryable<BoardThread> BoardThreads => context.BoardThreads
            .Include(x => x.Posts);

        /// <summary>
        /// The <see cref="IQueryable"/> collection of all the <see cref="BoardPost"/>s in the database.
        /// </summary>
        public IQueryable<BoardPost> BoardPosts => context.BoardPosts;

        /// <summary>
        /// The <see cref="IQueryable"/> collection of all the <see cref="NewsPost"/>s in the database, loaded with child <see cref="Comment"/>s.
        /// </summary>
        public IQueryable<NewsPost> NewsPosts => context.NewsPosts
            .Include(x=>x.Comments);

        public IQueryable<ColumnPost> ColumnPosts => context.ColumnPosts.Include(p => p.Author);

        public IQueryable<Author> Authors => context.Authors;

        public async Task<List<NewsPost>> GetLatestNewsPostsAsync(int count)
        {
            return await context.NewsPosts.OrderByDescending(x => x.DatePosted).Take(count).ToListAsync();
        }

        public async Task<NewsPageViewModel> GetNewsPageViewModelAsync()
        {
            List<NewsPost> recentPosts = await GetLatestNewsPostsAsync(25);
            List<NewsPost> popularPosts = recentPosts.OrderByDescending(x => x.ViewCount).ToList();

            while (popularPosts.Count < 7)
            {
                popularPosts.Add(new NewsPost());
            }

            NewsPageViewModel vm = new NewsPageViewModel()
            {
                MainPost = popularPosts[0],
                SecondaryPost = popularPosts[1],
                //TODO: Cache this, add related articles once when adding a new article
                MainPostRelatedPosts = recentPosts.Take(6).ToList(),
                PostList = popularPosts.GetRange(2, 5).ToList(),
                LatestNews = recentPosts.Take(6).ToList()
            };

            return vm;
        }


        /// <summary>
        /// Adds a new <see cref="Comment"/> to a <see cref="NewsPost"/>.
        /// </summary>
        /// <param name="comment">The comment to add.</param>
        /// <param name="postId">The id of the post which to add the comment to.</param>
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

        /// <summary>
        /// Adds a new <see cref="NewsPost"/> to the database.
        /// </summary>
        /// <param name="post">The post to add.</param>
        public void AddNewsPost(NewsPost post)
        {
            context.NewsPosts.Add(post);

            context.SaveChanges();
        }

        /// <summary>
        /// Updates a <see cref="NewsPost"/> in the database with the new values.
        /// </summary>
        /// <param name="post">The post to edit.</param>
        public void EditNewsPost(NewsPost post)
        {
            NewsPost postToEdit = context.NewsPosts.FirstOrDefault(x => x.Id == post.Id);

            postToEdit.Title = post.Title;
            postToEdit.Text = post.Text;
            postToEdit.Image = post.Image;

            context.SaveChanges();
        }

        /// <summary>
        /// Adds a new <see cref="BoardPost"/> to the database.
        /// </summary>
        /// <param name="post">The post to add.</param>
        public void AddPost(BoardPost post)
        {
            context.BoardPosts.Add(post);

            context.SaveChanges();
        }

        /// <summary>
        /// Adds a new <see cref="BoardPost"/> to a <see cref="BoardThread"/>.
        /// </summary>
        /// <param name="boardPost">The post to add.</param>
        /// <param name="threadId">The id of the thread which to add the post to.</param>
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

        /// <summary>
        /// Adds a new <see cref="BoardThread"/> to the database.
        /// </summary>
        /// <param name="thread">The thread to add.</param>
        public void AddThread(BoardThread thread)
        {
            context.BoardThreads.Add(thread);

            context.SaveChanges();
        }

        /// <summary>
        /// Removes a <see cref="NewsPost"/> from the database, along with all the child <see cref="Comment"/>s.
        /// </summary>
        /// <param name="post">The post to remove.</param>
        public void RemoveNewsPost(NewsPost post)
        {
            var comments = context.Comments.Where(x => post.Comments.Contains(x));

            context.Comments.RemoveRange(comments);

            context.NewsPosts.Remove(post);

            context.SaveChanges();
        }

        /// <summary>
        /// Removes a <see cref="BoardPost"/> from the database. Not implemented.
        /// </summary>
        /// <param name="post">The post to remove.</param>
        public void RemovePost(BoardPost post)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a <see cref="BoardThread"/> from the database, along with all the child <see cref="BoardPost"/>s 
        /// and their images stored locally.
        /// </summary>
        /// <param name="thread">The thread to remove.</param>
        public void RemoveThread(BoardThread thread)
        {
            var posts = context.BoardPosts.Where(x => thread.Posts.Contains(x));

            foreach (var post in posts)
            {
                WriterHelper.DeleteImage(post);
            }

            context.BoardPosts.RemoveRange(posts);

            context.BoardThreads.Remove(thread);

            context.SaveChanges();
        }

        /// <summary>
        /// Increases the <see cref="NewsPost.ViewCount"/> by 1.
        /// </summary>
        /// <param name="post">The post which view count to increment.</param>
        public void IncrementViewCount(NewsPost post)
        {
            NewsPost record = context.NewsPosts.FirstOrDefault(x=>x.Id == post.Id);
            record.ViewCount++;

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

        public void SaveAuthor(Author author)
        {
            var existing = context.Authors.FirstOrDefault(a => a.Id == author.Id);

            if (existing != null)
            {
                existing.Name = author.Name;
                existing.Bio = author.Bio;
                existing.Image = author.Image;
            }
            else
            {
                context.Authors.Add(author);
            }

            context.SaveChanges();
        }
    }
}
