using Bazirano.Models.Board;
using System.Linq;

namespace Bazirano.Models.DataAccess
{
    /// <summary>
    /// The interface defining <see cref="BoardPost"/> database access
    /// </summary>
    public interface IBoardPostRepository
    {
        /// <summary>
        /// The <see cref="IQueryable"/> collection of all the <see cref="BoardThread"/>s in the database.
        /// </summary>
        IQueryable<BoardPost> BoardPosts { get; }

        /// <summary>
        /// Adds a <see cref="BoardPost"/> to the database.
        /// </summary>
        /// <param name="post">The post to add.</param>
        void AddPost(BoardPost post);

        /// <summary>
        /// Removes a <see cref="BoardPost"/> from the database.
        /// </summary>
        /// <param name="post">The post to remove.</param>
        void RemovePost(BoardPost post);
    }
}
