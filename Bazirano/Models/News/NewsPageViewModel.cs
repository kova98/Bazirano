using System.Collections.Generic;

namespace Bazirano.Models.News
{
    /// <summary>
    /// The viewmodel class used for displaying the main news page.
    /// </summary>
    public class NewsPageViewModel
    {
        /// <summary>
        /// The main <see cref="Article"/>.
        /// </summary>
        public Article MainPost { get; set; }

        /// <summary>
        /// The list of posts related to the main post.
        /// </summary>
        public List<Article> MainPostRelatedPosts { get; set; }

        /// <summary>
        /// The second post displayed next to the main post.
        /// </summary>
        public Article SecondaryPost { get; set; }

        /// <summary>
        /// The list of posts displayed next to the second post. 
        /// </summary>
        public List<Article> PostList { get; set; }

        /// <summary>
        /// The list containing the latest <see cref="Article"/>s.
        /// </summary>
        public List<Article> LatestPosts { get; set; }
    }
}
