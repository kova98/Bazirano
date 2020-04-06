﻿using Bazirano.Scraper.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

            PostUrl = config["ScraperUrls:IndexHr"];
        }

        public async Task PostArticle()
        {
            if (ArticleQueue.Count == 0)
            {
                logger.LogInformation("No articles in queue to post, skipping...");

                return;
            }

            var article = ArticleQueue.Dequeue();

            logger.LogInformation($"Posting article... Source: {article.Source} - Title: {article.Title} - Guid: {article.Guid}");

            var json = JsonConvert.SerializeObject(article);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            await client.PostAsync(PostUrl, stringContent);
        }

        public async Task GetArticles()
        {
            foreach (var scraper in scrapers)
            {
                var articles = await scraper.GetArticlesAsync();

                foreach (var article in articles)
                {
                    if (postedArticlesRepo.ArticleExists(article) == false)
                    {
                        postedArticlesRepo.AddArticle(article);
                        ArticleQueue.Enqueue(article);
                    }
                }
            }
        }
    }
}
