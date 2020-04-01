using System.Collections.Generic;

namespace Bazirano.Scraper.Interfaces
{
    public interface IPostedArticlesRepository
    {
        List<Article> PostedArticles { get; set; }

        void AddArticle(Article article);

        bool ArticleHasNotBeenPosted(Article article);
    }
}