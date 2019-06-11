using Bazirano.Models.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.DataAccess
{
    interface INewsPostsRepository
    {
        IQueryable<NewsPost> NewsPosts { get; }
        void AddNewsPost(NewsPost post);
        void RemoveNewsPost(NewsPost post);
    }
}
