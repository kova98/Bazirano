using Bazirano.Models.Board;
using System.Linq;

namespace Bazirano.Models.DataAccess
{
    public interface IBoardPostsRepository
    {
        IQueryable<BoardPost> BoardPosts { get; }
        void AddPost(BoardPost post);
        void RemovePost(BoardPost post);
    }
}
