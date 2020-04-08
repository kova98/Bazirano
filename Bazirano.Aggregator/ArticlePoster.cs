using Bazirano.Aggregator.Interfaces;
using Bazirano.Aggregator.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bazirano.Aggregator
{
    class ArticlePoster
    {
        private readonly string PostUrl;
        private readonly IPostedArticlesRepository postedArticlesRepo;
        private readonly ILogger<ArticlePoster> logger;
        private readonly IEnumerable<IScraper> scrapers;

        private Queue<Article> ArticleQueue { get; set; } = new Queue<Article>();

        public ArticlePoster(ILogger<ArticlePoster> logger, IConfigurationRoot config, IPostedArticlesRepository postedArticlesRepo, IEnumerable<IScraper> scrapers)
        {
            this.postedArticlesRepo = postedArticlesRepo;
            this.logger = logger;
            this.scrapers = scrapers;

            PostUrl = config["ScraperUrls:PostUrl"];
        }

        public async Task PostArticle()
        {
            if (ArticleQueue.Count == 0)
            {
                logger.LogInformation("No articles in queue, fetching...");
                await GetArticles();

                return;
            }

            var article = ArticleQueue.Dequeue();

            var json = JsonConvert.SerializeObject(article);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            await client.PostAsync(PostUrl, stringContent);

            logger.LogInformation(
                $"Posted: {article.Source} - {article.Title} - " +
                $"Remaining in queue: {ArticleQueue.Count}");
        }

        public async Task GetArticles()
        {
            foreach (var scraper in scrapers)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var articles = await scraper.GetArticlesAsync();

                stopwatch.Stop();
                logger.LogInformation($"Fetched {articles.Count} from {articles[0].Source} in {stopwatch.Elapsed.TotalSeconds} seconds");

                foreach (var article in articles)
                {
                    var similiarArticle = postedArticlesRepo.FindSimiliarArticle(article);
                    if (similiarArticle == null)
                    {
                        postedArticlesRepo.AddArticle(article);
                        ArticleQueue.Enqueue(article);
                    }
                    else
                    {
                        logger.LogInformation(
                            $"Article '{article.Title}' from {article.Source} already posted or enqueued as " +
                            $"{article.Title} from {article.Source}. Skipping...");
                    }
                }
            }
        }
    }
}
