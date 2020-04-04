using Bazirano.Library.Enums;
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

        private IArticleRepository repository;

        public NewsHelper(IArticleRepository repo)
        {
            repository = repo;
        }

        public NewsPageViewModel GetNewsPageViewModel()
        {
            List<Article> recentPosts = repository.Articles.OrderByDescending(x => x.DatePosted).Take(25).ToList();
            List<Article> popularPosts = recentPosts.OrderByDescending(x => x.ViewCount).ToList();

            while (popularPosts.Count < 7)
            {
                popularPosts.Add(new Article());
            }

            var newsPageViewModel = new NewsPageViewModel()
            {
                MainPost = popularPosts[0],
                SecondaryPost = popularPosts[1],
                MainPostRelatedPosts = new List<Article>(), //recentPosts.Take(MainPostRelatedPostsCount).ToList(), //TODO: Implement related posts properly
                PostList = popularPosts.GetRange(2, PostListCount).ToList(),
                LatestPosts = recentPosts.Take(LatestPostsCount).ToList()
            };

            return newsPageViewModel;
        }

        public async Task<List<Article>> GetLatestArticlesAsync(int count)
        {
            return await repository.Articles.OrderByDescending(x => x.DatePosted).Take(count).ToListAsync();
        }

        public string GetNewsSourceDisplayName(NewsSource source)
            => source switch
            {
                NewsSource.Unknown => "Nepoznato",
                NewsSource.IndexHr => "Index",

                _ => throw new ArgumentException(message: "invalid enum value", paramName: "source")
                
            };
    }
}
