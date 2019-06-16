using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Bazirano.Models.News
{
    public class NewsPost
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Summary { get; set; }
        public DateTime DatePosted { get; set; }
        public ICollection<NewsComment> Comments { get; set; }
        public string Keywords
        {
            get { return KeywordsList == null ? "" : string.Join(",", KeywordsList); }
            set { KeywordsList = value?.Split(',').ToList(); }
        }

        [NotMapped]
        public ICollection<string> KeywordsList { get; set; }
    }
}
