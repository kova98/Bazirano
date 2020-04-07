using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bazirano.Aggregator.Interfaces
{
    public interface IScraper
    {
        Task<List<Article>> GetArticlesAsync();
    }
}