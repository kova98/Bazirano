using Bazirano.Scraper.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Bazirano.Scraper
{
    public sealed class InMemoryPostedArticlesRepository : IPostedArticlesRepository
    {
        private const int MaxPostedArticles = 50;
        
        public List<Article> PostedArticles { get; private set; } = new List<Article>();

        public void AddArticle(Article article)
        {
            if (ArticleExists(article) == false)
            {
                PostedArticles.Add(article);
                if (PostedArticles.Count > MaxPostedArticles)
                {
                    PostedArticles.RemoveAt(0);
                }
            }
        }

        private bool ArticleExists(Article article)
        {
            foreach (var posted in PostedArticles)
            {
                var keywordMatches = posted.KeywordsList.Intersect(article.KeywordsList).Count();

                if (keywordMatches > 5)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
