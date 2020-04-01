using System.Collections.Generic;

namespace Bazirano.Scraper.Interfaces
{
    public interface IPostedArticlesRepository
    {
        void AddArticle(Article article);
    }
}