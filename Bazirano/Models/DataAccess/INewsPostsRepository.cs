using Bazirano.Models.News;
using Bazirano.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.DataAccess
{
    public interface IArticleRepository
    {
        /// <summary>
        /// The <see cref="IQueryable"/> collection of all the <see cref="Article"/>s in the database.
        /// </summary>
        IQueryable<Article> Articles { get; }

        /// <summary>
        /// Adds a new <see cref="Article"/> to the database.
        /// </summary>
        /// <param name="post">The post to add.</param>
        void AddArticle(Article article);

        /// <summary>
        /// Removes an <see cref="Article"/> from the database.
        /// </summary>
        /// <param name="post">The post to remove.</param>
        void RemoveArticle(long articleId);

        /// <summary>
        /// Updates a <see cref="Article"/> in the database with the new values.
        /// </summary>
        /// <param name="post">The post to edit.</param>
        void EditArticle(Article article);

        /// <summary>
        /// Adds a new <see cref="Comment"/> to a <see cref="Article"/>.
        /// </summary>
        /// <param name="comment">The comment to add.</param>
        /// <param name="postId">The id of the post which to add the comment to.</param>
        void AddCommentToArticle(Comment comment, long postId);

        /// <summary>
        /// Increases the <see cref="Article.ViewCount"/> by 1.
        /// </summary>
        /// <param name="post">The post which view count to increment.</param>
        void IncrementViewCount(Article post);       
    }
}
