using System.Threading.Tasks;

namespace Bazirano.Scraper.Interfaces
{
    public interface IScraper
    {
        Task<Article> GetArticleAsync();
    }
}