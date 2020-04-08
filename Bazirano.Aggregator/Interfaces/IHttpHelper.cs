using AngleSharp.Dom;
using System.Threading.Tasks;

namespace Bazirano.Aggregator.Interfaces
{
    public interface IHttpHelper
    {
        Task<string> Get(string url);

        Task<IDocument> GetDocumentFromUrl(string url);

        Task<IDocument> GetDocumentFromHtml(string html);

        void Post(string url, string body);

    }
}