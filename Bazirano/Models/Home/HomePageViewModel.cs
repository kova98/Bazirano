using Bazirano.Models.News;
using Bazirano.Models.Board;
using System.Collections.Generic;

namespace Bazirano.Models.Home
{
    /// <summary>
    /// The viewmodel class used for displaying all home page elements.
    /// </summary>
    public class HomePageViewModel
    {
        /// <summary>
        /// The main post to display.
        /// </summary>
        public NewsPost MainPost { get; set; }

        /// <summary>
        /// The list of popular posts to display.
        /// </summary>
        public List<NewsPost> PopularPosts { get; set; }

        /// <summary>
        /// The list of board threads to display.
        /// </summary>
        public List<BoardThread> Threads { get; set; }
    }
}
