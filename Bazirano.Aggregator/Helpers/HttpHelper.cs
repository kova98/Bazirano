using AngleSharp;
using AngleSharp.Dom;
using Bazirano.Aggregator.Interfaces;
using System;
using System.Net.Http;
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

        public void Post(string url, string body)
        {
            throw new NotImplementedException();
        }
    }
}
