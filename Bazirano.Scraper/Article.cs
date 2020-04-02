using Bazirano.Scraper.Enums;
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
        public long Id { get; set; }

        [Range(1, long.MaxValue)]
        public long Guid { get; set; }

        public long ViewCount { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public string Summary { get; set; }

        public DateTime DatePosted { get; set; }

        [Required]
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
