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
            
            var recentPosts = await repository.GetLatestNewsPosts(25);

            while (recentPosts.Count < 7)
            {
                recentPosts.Add(new NewsPost());

                //TODO: Handle this properly
            }

            var popularRecentPosts = recentPosts.OrderByDescending(x => x.ViewCount).ToList();

            vm.MainPost = popularRecentPosts[0];
            vm.SecondaryPost = popularRecentPosts[1];

            //TODO: Cache this, add related articles once when adding a new article
            vm.MainPostRelatedPosts = recentPosts.Take(6).ToList();

            //var query = repository.NewsPosts
            //    .Where(p => p.KeywordsList.KeywordMatches(vm.MainPost.KeywordsList) > 5 && p.Id != vm.MainPost.Id)
            //    .OrderByDescending(p => p.KeywordsList.KeywordMatches(vm.MainPost.KeywordsList));

            vm.PostList = popularRecentPosts.GetRange(2, 5).ToList();

            vm.LatestNews = recentPosts.Take(6).ToList();

            return vm;
        }

        public static TimeDisplay GetTimeDisplayFromTimeElapsed(TimeSpan elapsed)
        {
            int timeNumber;
            string timeText;

            if (elapsed.Days > 0)
            {
                timeNumber = elapsed.Days;
                timeText = GetDayNoun(elapsed.Days);
            }
            else if (elapsed.Hours > 0)
            {
                timeNumber = elapsed.Hours;
                timeText = GetHourNoun(elapsed.Hours);
            }
            else
            {
                timeNumber = elapsed.Minutes;
                timeText = GetMinuteNoun(elapsed.Minutes);
            }

            return new TimeDisplay { Number = timeNumber, Text = timeText };
        }

        private static string GetHourNoun(int hours)
        {
            char lastDigit = hours.ToString().Last();

            if (lastDigit == '1' && hours != 11)
            {
                return "sat";
            }
            if ((lastDigit == '2' || lastDigit == '3' || lastDigit == '4') && (hours > 14 || hours < 10))
            {
                return "sata";
            }

            return "sati";
        }

        private static string GetDayNoun(int days)
        {
            char lastDigit = days.ToString().Last();
            return (lastDigit == '1' && days != 11) ? "dan" : "dana";
        }

        private static string GetMinuteNoun(int minutes)
        {
            return "min";
        }

    }
}
