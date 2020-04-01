using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bazirano.Scraper
{
    public class Program
    {
        private const string postUrl = "https://localhost:44326/api/postNews";

        private static async Task Main(string[] args)
        {
            var repo = new InMemoryPostedArticlesRepository();
            var httpHelper = new HttpHelper();
            var scraper = new IndexHrScraper(repo, httpHelper);

            while (true)
            {
                try
                {
                    await Loop(scraper);
                    await Task.Delay(10000);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"[Error] {DateTime.Now}");
                    Console.WriteLine($"An exception occured: {e.Message}");
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        private static async Task Loop(IndexHrScraper scraper)
        {
            var article = await scraper.GetArticleAsync();

            if (article != null)
            {
                Console.WriteLine($"Posting article...");
                Console.WriteLine($"Title: {article.Title}");
                Console.WriteLine($"Guid: {article.Guid}");

                var json = JsonConvert.SerializeObject(article);
                await PostArticle(json);
            }
        }

        private static async Task PostArticle(string json)
        {
            using (var client = new HttpClient())
            {
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync(postUrl, stringContent);
            }
        }
    }
}
