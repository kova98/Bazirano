using Bazirano.Models.Board;
using Bazirano.Models.News;
using System.Linq;

namespace Bazirano.Models.DataAccess
{
    public interface INewsPostsRepository
    {
        IQueryable<NewsPost> NewsPosts { get; }
        void AddNewsPost(NewsPost post);
        void RemoveNewsPost(NewsPost post);
        void AddCommentToNewsPost(NewsComment comment, long postId);
        void IncrementViewCount(NewsPost post);
    }
}
