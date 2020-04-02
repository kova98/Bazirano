using Bazirano.Scraper.Helpers;
using System;
using System.Threading.Tasks;

namespace Bazirano.Scraper
{
    public class Program
    {
        private const int WorkCycleFrequencyInSeconds = 3;

        private static ArticlePoster articlePoster;

        private static async Task Main(string[] args)
        {
            var repo = new InMemoryPostedArticlesRepository();
            var httpHelper = new HttpHelper();

            articlePoster = new ArticlePoster
            (
                repo,
                new IndexHrScraper(httpHelper)
            );

            while (true)
            {
                try
                {
                    await DoWork();
                    await Task.Delay(WorkCycleFrequencyInSeconds * 1000);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"[Error] {DateTime.Now}");
                    Console.WriteLine($"An exception occured: {e.Message}");
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        private static async Task DoWork()
        {
            await articlePoster.PostArticle();
            await articlePoster.GetArticles();
        }
    }
}
