using AngleSharp.Dom;
using System.Threading.Tasks;

namespace Bazirano.Scraper.Interfaces
{
    public interface IHttpHelper
    {
        Task<string> Get(string url);

        Task<IDocument> GetAsDocument(string url);

        void Post(string url, string body);

    }
}