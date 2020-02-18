using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public class NewsHelper
    {
        private const int PostListCount = 5;
        private const int LatestPostsCount = 6;
        private const int MainPostRelatedPostsCount = 6;

        private INewsPostsRepository repository;

        public NewsHelper(INewsPostsRepository repo)
        {
            repository = repo;
        }

        public NewsPageViewModel GetNewsPageViewModel()
        {
            List<NewsPost> recentPosts = repository.NewsPosts.OrderByDescending(x => x.DatePosted).Take(25).ToList();
            List<NewsPost> popularPosts = recentPosts.OrderByDescending(x => x.ViewCount).ToList();

            while (popularPosts.Count < 7)
            {
                popularPosts.Add(new NewsPost());
            }

            var newsPageViewModel = new NewsPageViewModel()
            {
                MainPost = popularPosts[0],
                SecondaryPost = popularPosts[1],
                MainPostRelatedPosts = recentPosts.Take(MainPostRelatedPostsCount).ToList(), //TODO: Implement related posts properly
                PostList = popularPosts.GetRange(2, PostListCount).ToList(),
                LatestPosts = recentPosts.Take(LatestPostsCount).ToList()
            };

            return newsPageViewModel;
        }

        public async Task<List<NewsPost>> GetLatestNewsPostsAsync(int count)
        {
            return await repository.NewsPosts.OrderByDescending(x => x.DatePosted).Take(count).ToListAsync();
        }
    }
}
