using Bazirano.Library.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Bazirano.Scraper
{
    /// <summary>
    /// The news post model.
    /// </summary>
    public class Article
    {
        public long Guid { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public string Image { get; set; }

        public string Summary { get; set; }

        public string Keywords
        {
            get { return KeywordsList == null ? "" : string.Join(",", KeywordsList); }
            set { KeywordsList = value?.Split(',').ToList(); }
        }

        [NotMapped]
        public ICollection<string> KeywordsList { get; set; }

        public NewsSources Source { get; set; }
    }
}
