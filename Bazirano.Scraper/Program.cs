using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bazirano.Scraper
{
    public class Program
    {
        private const string postUrl = "https://localhost:44326/api/postNews";

        private static async Task Main(string[] args)
        {
            var scraper = new IndexHrScraper();

            while (true)
            {
                await Loop(scraper);
                await Task.Delay(10000);
            }
        }

        private static async Task Loop(IndexHrScraper scraper)
        {
            var article = await scraper.GetArticle();

            if (article != null)
            {
                Console.WriteLine($"Posting article...");
                Console.WriteLine($"Title: {article.Title}");
                Console.WriteLine($"Guid: {article.Guid}");

                var json = JsonConvert.SerializeObject(article);

                using (var client = new HttpClient())
                {
                    // POST the article (to be implemented)
                    //await client.PostAsync(postUrl)
                }
            }
        }   
    }
}
