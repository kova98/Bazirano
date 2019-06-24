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
            return repository.NewsPosts
                .OrderByDescending(x => x.DatePosted).Take(15)
                .OrderByDescending(x => x.ViewCount).First();
        }

        private List<NewsPost> GetMainPostRelatedPosts(NewsPost mainPost)
        {
            return repository.NewsPosts
                .Where(p => p.KeywordsList.KeywordMatches(mainPost.KeywordsList) > 0)
                .Where(p=> p.Id != mainPost.Id)
                .OrderByDescending(p=> p.KeywordsList.KeywordMatches(mainPost.KeywordsList))
                .ToList();
        }

        private NewsPost GetSecondaryPost()
        {
            return repository.NewsPosts
                .OrderByDescending(x => x.DatePosted).Take(15)
                .OrderByDescending(x => x.ViewCount).ToList()[1];
        }

        private List<NewsPost> GetPostList()
        {
            return repository.NewsPosts
                .OrderByDescending(x => x.DatePosted).Take(15)
                .OrderByDescending(x => x.ViewCount).ToList().GetRange(2,5);
        }

        private List<NewsPost> GetLatestNews()
        {
            return repository.NewsPosts.OrderByDescending(x => x.DatePosted).Take(6).ToList();
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
