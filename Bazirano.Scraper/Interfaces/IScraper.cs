using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bazirano.Scraper.Interfaces
{
    public interface IScraper
    {
        Task<List<Article>> GetArticlesAsync();
    }
}