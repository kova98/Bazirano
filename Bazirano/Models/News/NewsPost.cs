using System;
using System.Collections.Generic;

namespace Bazirano.Models.News
{
    public class NewsPost
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public DateTime DatePosted { get; set; }
        public ICollection<NewsComment> Comments { get; set; }
    }
}
