using Bazirano.Scraper.Helpers;
using Bazirano.Scraper.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bazirano.Scraper
{
    public class Program
    {
        private const int WorkCycleFrequencyInSeconds = 3;

        private static ArticlePoster articlePoster;

        private static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            articlePoster = serviceProvider.GetService<ArticlePoster>();

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

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole());

            services.AddTransient<ArticlePoster>();
            services.AddTransient<IPostedArticlesRepository, InMemoryPostedArticlesRepository>();
            services.AddTransient<IHttpHelper, HttpHelper>();

            services.AddTransient<IScraper, IndexHrScraper>();

            services.AddSingleton(
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .Build());

        }

        private static async Task DoWork()
        {
            await articlePoster.PostArticle();
            await articlePoster.GetArticles();
        }
    }
}
