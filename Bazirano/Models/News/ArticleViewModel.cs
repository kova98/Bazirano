using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.News
{
    public class ArticleViewModel
    {
        public NewsPost Article { get; set; }
        public List<NewsPost> LatestNews { get; set; }

        public string[] ArticleSentences => Article.Text.Split('~');
    }
}
