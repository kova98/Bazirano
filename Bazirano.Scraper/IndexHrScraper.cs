using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using Microsoft.Toolkit.Parsers.Rss;
using Markdig;
using System.Text;

namespace Bazirano.Scraper
{
    internal class IndexHrScraper
    {
        string url = "https://www.index.hr/rss/najcitanije";
        Article lastArticle;

        public async Task<Article> GetArticle()
        {
            var article = await ScraperGetArticle();

            if (lastArticle == null || article.Guid != lastArticle.Guid)
            {
                lastArticle = article;
                return article;
            } 
            else
            {
                return null;
            }
        }

        private async Task<Article> ScraperGetArticle()
        {
            string feed = "";

            using (var client = new HttpClient())
            {
                feed = await client.GetStringAsync(url);
            }

            if (string.IsNullOrEmpty(feed))
            {
                throw new NullReferenceException($"Could not get feed from url {url}");
            }

            var parser = new RssParser();
            var rss = parser.Parse(feed);
            var newestArticle = rss.OrderByDescending(x => x.PublishDate).First();

            var article = new Article
            {
                Guid = GetGuid(newestArticle.InternalID),
                Title = newestArticle.Title,
                Image = newestArticle.ImageUrl,
                Text = await GetArticleText(newestArticle.InternalID),
                Summary = newestArticle.Summary,
                Keywords = GetKeywords(newestArticle.Title),
                DatePosted = DateTime.Now,
            };

            return article;
        }

        private string GetKeywords(string title)
        {
            return "";
        }

        private async Task<string> GetArticleText(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);
            var paragraphs = document.QuerySelectorAll("div.text > p");

            var stringBuilder = new StringBuilder();
            foreach (var par in paragraphs)
            {
                stringBuilder.Append(par.TextContent + Environment.NewLine + Environment.NewLine);
            }

            return stringBuilder.ToString();

            var stringResult = Markdown.ToHtml(stringBuilder.ToString());

            return null;
        }

        private long GetGuid(string internalID)
        {
            var index = internalID.IndexOf("id=") + 3;
            var guid = internalID.Substring(index);

            return Convert.ToInt64(guid);
        }

        private bool IsWordValid(string word)
        {
            if (word.Length < 4 ||
                word == "FOTO" ||
                word == "VIDEO")
            {
                return false;
            }

            return true;
        }
    }
}