using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.News
{
    public class NewsPageViewModel
    {
        public NewsPost MainPost { get; set; }
        public List<NewsPost> MainPostRelatedPosts { get; set; }

        public NewsPost SecondaryPost { get; set; }
        public List<NewsPost> PostList { get; set; }

        public List<NewsPost> LatestNews { get; set; }
    }
}
