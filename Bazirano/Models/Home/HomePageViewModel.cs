using Bazirano.Models.News;
using Bazirano.Models.Board;
using System.Collections.Generic;

namespace Bazirano.Models.Home
{
    public class HomePageViewModel
    {
        public NewsPost MainPost { get; set; }
        public List<NewsPost> PopularPosts { get; set; }
        public List<BoardThread> Threads { get; set; }
    }
}
