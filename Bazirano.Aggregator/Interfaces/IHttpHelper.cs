using AngleSharp.Dom;
using System.Threading.Tasks;

namespace Bazirano.Aggregator.Interfaces
{
    public interface IHttpHelper
    {
        Task<string> Get(string url);

        Task<IDocument> GetDocumentFromUrl(string url);

        Task<IDocument> GetDocumentFromHtml(string html);

        Task<string> GetFirstParagraph(string html);

        Task<string> GetArticleText(string html);
    }
}