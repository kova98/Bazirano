using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Parsers.Rss;
using System.Text;
using System.Collections.Generic;
using Bazirano.Aggregator.Interfaces;
using Bazirano.Aggregator.Helpers;
using Bazirano.Library.Enums;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Bazirano.Aggregator.Models;

namespace Bazirano.Aggregator.Scrapers
{
    public class KonzervaHrScraper : IScraper
    {
        private const string FeedUrl = "http://konzerva.hr/category/vijesti/feed/";

        private readonly IHttpHelper httpHelper;
        private readonly KeywordHelper keywordHelper;
         
        public KonzervaHrScraper(IHttpHelper httpHelper)
        {
            this.httpHelper = httpHelper;

            keywordHelper = new KeywordHelper();
        }

        public async Task<List<Article>> GetArticlesAsync()
        {
            var articles = await ScrapeArticles();

            return articles;
        }

        private async Task<List<Article>> ScrapeArticles()
        {
            var articles = new List<Article>();

            var feed = await httpHelper.Get(FeedUrl);

            if (string.IsNullOrEmpty(feed))
            {
                throw new NullReferenceException($"Could not get feed from url {FeedUrl}");
            }

            var parser = new RssParser();
            var rss = parser.Parse(feed);
            var newestArticlesSchemas = rss.OrderByDescending(x => x.PublishDate).Take(10);

            foreach (var schema in newestArticlesSchemas)
            {
                var document = await httpHelper.GetDocumentFromUrl(schema.InternalID);
                var articleText = GetArticleText(document);
                var summary = await httpHelper.GetFirstParagraph(schema.Content);
                var keywords = keywordHelper.GetKeywordsFromTitle(schema.Title);

                articles.Add(new Article
                {
                    Source = NewsSource.KonzervaHr,
                    Guid = GetGuidFromUrl(schema.InternalID),
                    Title = schema.Title,
                    Image = GetArticleImage(document),
                    Text = articleText,
                    Summary = summary,
                    Keywords = keywords,
                    SourceUrl = schema.InternalID
                });
            }

            return articles;
        }

        private string GetArticleImage(IDocument document)
        {
            var img = document.QuerySelector(".td-modal-image");
            var imgUrl = img.GetAttribute("src");

            return imgUrl;
        }

        private string GetArticleText(IDocument document)
        {
            var paragraphs = document
                .QuerySelectorAll("p")
                .Where(x =>
                    x.ClassList.Count() == 0 &&
                    x.Children.Any(x => x is IHtmlScriptElement) == false);

            var paragraphStringBuilder = new StringBuilder();
            foreach (var par in paragraphs)
            {
                paragraphStringBuilder.Append(par.TextContent + Environment.NewLine + Environment.NewLine);
            }

            return paragraphStringBuilder.ToString();
        }

        private long GetGuidFromUrl(string url)
        {
            var index = url.IndexOf("?p=") + 3;
            var guid = url.Substring(index);

            return Convert.ToInt64(guid);
        }
    }
}