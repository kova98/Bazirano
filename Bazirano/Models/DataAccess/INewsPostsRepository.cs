using Bazirano.Models.News;
using Bazirano.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.DataAccess
{
    public interface INewsPostsRepository
    {
        /// <summary>
        /// The <see cref="IQueryable"/> collection of all the <see cref="NewsPost"/>s in the database.
        /// </summary>
        IQueryable<NewsPost> NewsPosts { get; }

        Task<List<NewsPost>> GetLatestNewsPosts(int count);

        /// <summary>
        /// Adds a new <see cref="NewsPost"/> to the database.
        /// </summary>
        /// <param name="post">The post to add.</param>
        void AddNewsPost(NewsPost post);

        /// <summary>
        /// Removes a <see cref="NewsPost"/> from the database.
        /// </summary>
        /// <param name="post">The post to remove.</param>
        void RemoveNewsPost(NewsPost post);

        /// <summary>
        /// Updates a <see cref="NewsPost"/> in the database with the new values.
        /// </summary>
        /// <param name="post">The post to edit.</param>
        void EditNewsPost(NewsPost post);

        /// <summary>
        /// Adds a new <see cref="Comment"/> to a <see cref="NewsPost"/>.
        /// </summary>
        /// <param name="comment">The comment to add.</param>
        /// <param name="postId">The id of the post which to add the comment to.</param>
        void AddCommentToNewsPost(Comment comment, long postId);

        /// <summary>
        /// Increases the <see cref="NewsPost.ViewCount"/> by 1.
        /// </summary>
        /// <param name="post">The post which view count to increment.</param>
        void IncrementViewCount(NewsPost post);
    }
}
