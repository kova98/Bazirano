using System.Collections.Generic;

namespace Bazirano.Aggregator.Interfaces
{
    public interface IPostedArticlesRepository
    {
        void AddArticle(Article article);

        Article FindSimiliarArticle(Article article);
    }
}