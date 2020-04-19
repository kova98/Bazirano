using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bazirano.Aggregator.Interfaces;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bazirano.Aggregator.Helpers
{
    class HttpHelper : IHttpHelper
    {
        public async Task<string> Get(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                return response;
            }
        }

        public async Task<string> GetArticleText(string html)
        {
            var document = await GetDocumentFromHtml(html);

            var elements = document.QuerySelectorAll("p");

            var stringBuilder = new StringBuilder();
            foreach (var element in elements)
            {
                if (element.IsLastChild())
                {
                    break;
                }
                else if (element is IHtmlTitleElement)
                {
                    stringBuilder.Append($"#{element.TextContent}");
                }
                else if (element is IHtmlParagraphElement)
                {
                    stringBuilder.Append(element.TextContent);
                }

                stringBuilder.Append(Environment.NewLine + Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        public async Task<IDocument> GetDocumentFromHtml(string html)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            return await context.OpenAsync(res => res.Content(html));
        }

        public async Task<IDocument> GetDocumentFromUrl(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            
            return await context.OpenAsync(url);
        }

        public async Task<string> GetFirstParagraph(string html)
        {
            var document = await GetDocumentFromHtml(html);

            var element = document.QuerySelector("p");

            return element.TextContent;
        }
    }
}
