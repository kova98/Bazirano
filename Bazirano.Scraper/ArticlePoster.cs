using Bazirano.Scraper.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bazirano.Scraper
{
    class ArticlePoster
    {
        private const string PostUrl = "https://localhost:44326/api/postNews";

        private Queue<Article> ArticleQueue { get; set; } = new Queue<Article>();
        private List<IScraper> Scrapers { get; set; } = new List<IScraper>();

        public ArticlePoster(params IScraper[] scrapers)
        {
            Scrapers.AddRange(scrapers);
        }

        public async Task PostArticle()
        {
            if (ArticleQueue.Count == 0)
            {
                Console.WriteLine($"No articles in queue to post, skipping...");
                return;
            }

            var article = ArticleQueue.Dequeue();

            Console.WriteLine($"Posting article...");
            Console.WriteLine($"Title: {article.Title}");
            Console.WriteLine($"Guid: {article.Guid}");

            var json = JsonConvert.SerializeObject(article);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            await client.PostAsync(PostUrl, stringContent);
        }

        public async Task GetArticles()
        {
            foreach (var scraper in Scrapers)
            {
                var article = await scraper.GetArticleAsync();

                ArticleQueue.Enqueue(article);
            }
        }
    }
}
