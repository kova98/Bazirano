using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using Microsoft.EntityFrameworkCore;

namespace Bazirano.Infrastructure
{
    // TODO: Look into refactoring this. Possibly the cause of error: 
    // A second operation started on this context before a previous operation completed.
    public class NewsHelper : INewsHelper
    {
        private INewsPostsRepository repository;

        public NewsHelper(INewsPostsRepository repo)
        {
            repository = repo;
        }

        public async Task<NewsPageViewModel> GetCurrentNewsAsync()
        {
            NewsPageViewModel vm = new NewsPageViewModel();

            var recentPosts = await GetLastXPosts(50);

            var popularRecentPosts = recentPosts.OrderByDescending(x => x.ViewCount).ToList();

            if (popularRecentPosts.Count == 0)
            {
                //TODO: Handle this somehow
            }

            vm.MainPost = popularRecentPosts[0];
            vm.SecondaryPost = popularRecentPosts[1];

            //TODO: Cache this, add related articles once when adding a new article
            vm.MainPostRelatedPosts = recentPosts.Take(6).ToList();

            var query = repository.NewsPosts
                .Where(p => p.KeywordsList.KeywordMatches(vm.MainPost.KeywordsList) > 5 && p.Id != vm.MainPost.Id)
                .OrderByDescending(p => p.KeywordsList.KeywordMatches(vm.MainPost.KeywordsList));

            vm.PostList = popularRecentPosts.GetRange(2, 5).ToList();

            vm.LatestNews = recentPosts.Take(6).ToList();

            return vm;
        }

        public async Task<List<NewsPost>> GetLastXPosts(int x)
        {
            var recentPosts = await repository.NewsPosts
                .Where(p => repository.NewsPosts.LongCount() - p.Id < x) // Last x posts
                .OrderByDescending(p => p.DatePosted).ToListAsync();

            return recentPosts;
        }

        public static TimeDisplay GetTimeElapsed(TimeSpan elapsed)
        {
            int timeNumber;
            string timeText;

            if (elapsed.Days > 0)
            {
                timeNumber = elapsed.Days;

                string daysString = timeNumber.ToString();
                char lastDigit = daysString[daysString.Length - 1];
                timeText = (lastDigit == '1' && elapsed.Days != 11) ? "dan" : "dana";
            }
            else if (elapsed.Hours > 0)
            {
                timeNumber = elapsed.Hours;

                string hoursString = timeNumber.ToString();
                char lastDigit = hoursString[hoursString.Length - 1];
                if (lastDigit == '1' && elapsed.Hours != 11)
                {
                    timeText = "sat";
                }
                else if (lastDigit == '2' || lastDigit == '3' || lastDigit == '4')
                {
                    timeText = "sata";
                }
                else
                {
                    timeText = "sati";
                }
            }
            else
            {
                timeNumber = elapsed.Minutes;

                string minutesString = elapsed.Minutes.ToString();
                char lastDigit = minutesString[minutesString.Length - 1];
                timeText = "min";
            }

            return new TimeDisplay { Number = timeNumber, Text = timeText };
        }
    }
}
