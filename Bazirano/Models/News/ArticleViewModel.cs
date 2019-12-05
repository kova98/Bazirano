using System.Collections.Generic;

namespace Bazirano.Models.News
{
    /// <summary>
    /// The viewmodel class used for displaying a <see cref="NewsPost"/>
    /// </summary>
    public class ArticleViewModel
    {
        /// <summary>
        /// The <see cref="NewsPost"/> to display.
        /// </summary>
        public NewsPost Article { get; set; }

        /// <summary>
        /// The list of latest <see cref="NewsPost"/>s to display on the side.
        /// </summary>
        public List<NewsPost> LatestNews { get; set; }

        /// <summary>
        /// The property splitting the article text into sentences ^
        /// </summary>
        public string[] ArticleSentences => Article.Text.Split('~');
    }
}
