using System.Collections.Generic;

namespace Bazirano.Models.News
{
    /// <summary>
    /// The viewmodel class used for displaying the main news page.
    /// </summary>
    public class NewsPageViewModel
    {
        /// <summary>
        /// The main <see cref="NewsPost"/>.
        /// </summary>
        public NewsPost MainPost { get; set; }

        /// <summary>
        /// The list of posts related to the main post.
        /// </summary>
        public List<NewsPost> MainPostRelatedPosts { get; set; }

        /// <summary>
        /// The second post displayed next to the main post.
        /// </summary>
        public NewsPost SecondaryPost { get; set; }

        /// <summary>
        /// The list of posts displayed next to the second post. 
        /// </summary>
        public List<NewsPost> PostList { get; set; }

        /// <summary>
        /// The list containing the latest <see cref="NewsPost"/>s.
        /// </summary>
        public List<NewsPost> LatestPosts { get; set; }
    }
}
