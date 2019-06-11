using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;

namespace Bazirano.Infrastructure
{
    public class NewsHelper : INewsHelper
    {
        private INewsPostsRepository repository;

        public NewsHelper(INewsPostsRepository repo)
        {
            repository = repo;
        }

        public NewsPageViewModel CurrentNews => new NewsPageViewModel
        {
            MainPost = GetMainPost(),
            MainPostRelatedPosts = GetMainPostRelatedPosts(GetMainPost()),
            SecondaryPost = GetSecondaryPost(),
            PostList = GetPostList(),
            LatestNews = GetLatestNews()
        };

        private NewsPost GetMainPost()
        {
            return repository.NewsPosts.First();
        }

        private List<NewsPost> GetMainPostRelatedPosts(NewsPost mainPost)
        {
            return repository.NewsPosts.Where(p => p.Title.Contains("related")).ToList();
        }

        private NewsPost GetSecondaryPost()
        {
            return repository.NewsPosts.ToList()[2];
        }

        private List<NewsPost> GetPostList()
        {
            return repository.NewsPosts.Take(5).ToList();
        }

        private List<NewsPost> GetLatestNews()
        {
            return repository.NewsPosts
                .Where(x => x.DatePosted.LessThanHourElapsed())
                .OrderByDescending(x => x.DatePosted)
                .ToList();
        }
    }
}
