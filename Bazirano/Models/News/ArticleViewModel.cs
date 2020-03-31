using System.Collections.Generic;

namespace Bazirano.Models.News
{
    /// <summary>
    /// The viewmodel class used for displaying a <see cref="News.Article"/>
    /// </summary>
    public class ArticleViewModel
    {
        /// <summary>
        /// The <see cref="News.Article"/> to display.
        /// </summary>
        public Article Article { get; set; }

        /// <summary>
        /// The list of latest <see cref="News.Article"/>s to display on the side.
        /// </summary>
        public List<Article> LatestNews { get; set; }

        /// <summary>
        /// The property splitting the article text into paragraphs
        /// </summary>
        public string[] Paragraphs => Article.Text.Split('\n');
    }
}
