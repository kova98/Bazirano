using Bazirano.Aggregator.Helpers;
using Bazirano.Aggregator.Interfaces;
using Bazirano.Aggregator.Scrapers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bazirano.Aggregator
{
    public class Program
    {
        private static ArticlePoster articlePoster;

        private static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            articlePoster = serviceProvider.GetService<ArticlePoster>();

            var config = serviceProvider.GetService<IConfigurationRoot>();
            var logger = serviceProvider.GetService<ILogger<Program>>();

            while (true)
            {
                var succeeded = int.TryParse(config["WorkCycleDelayInSeconds"], out int delayInSeconds);
                if (succeeded == false)
                {
                    delayInSeconds = 60;
                    logger.LogWarning("WorkCycleDelayInSeconds parsing from config file failed. Using default value (60)");
                }

                var delayInMiliseconds = delayInSeconds * 1000;

                await DoWork();
                await Task.Delay(delayInMiliseconds);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole());

            services.AddTransient<ArticlePoster>();
            services.AddTransient<IPostedArticlesRepository, InMemoryPostedArticlesRepository>();
            services.AddTransient<IHttpHelper, HttpHelper>();

            //services.AddTransient<IScraper, IndexHrScraper>();
            //services.AddTransient<IScraper, KonzervaHrScraper>();
            services.AddTransient<IScraper, PriznajemHrScraper>();

            services.AddSingleton(
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .Build());
        }

        private static async Task DoWork()
        {
            await articlePoster.PostArticle();
        }
    }
}