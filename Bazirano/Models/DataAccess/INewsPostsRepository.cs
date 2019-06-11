using Bazirano.Models.News;
using System.Linq;

namespace Bazirano.Models.DataAccess
{
    public interface INewsPostsRepository
    {
        IQueryable<NewsPost> NewsPosts { get; }
        void AddNewsPost(NewsPost post);
        void RemoveNewsPost(NewsPost post);
    }
}
