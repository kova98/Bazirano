using Bazirano.Models.Board;
using System.Linq;

namespace Bazirano.Models.DataAccess
{
    /// <summary>
    /// The interface defining <see cref="BoardThread"/> database access
    /// </summary>
    public interface IBoardThreadsRepository
    {
        /// <summary>
        /// The <see cref="IQueryable"/> collection of all the <see cref="BoardThread"/>s in the database.
        /// </summary>
        IQueryable<BoardThread> BoardThreads { get; }

        /// <summary>
        /// Adds a new <see cref="BoardThread"/> to the database.
        /// </summary>
        /// <param name="thread">The thread to add.</param>
        void AddThread(BoardThread thread);

        /// <summary>
        /// Removes a <see cref="BoardThread"/> from the database.
        /// </summary>
        /// <param name="thread">The thread to remove.</param>
        void RemoveThread(long id);

        /// <summary>
        /// Adds a new <see cref="BoardPost"/> to a <see cref="BoardThread"/>.
        /// </summary>
        /// <param name="boardPost">The post to add.</param>
        /// <param name="threadId">The id of the thread which to add the post to.</param>
        void AddPostToThread(BoardPost boardPost, long threadId);
    }
}
