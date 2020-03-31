using Bazirano.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Bazirano.Models.News
{
    /// <summary>
    /// The news post model.
    /// </summary>
    public class Article
    {
        /// <summary>
        /// The id of the post. Used as the primary key in the database.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The third party guid of the post. This comes from the scraper and is used for identifying duplicate posts.
        /// </summary>
        [Range(1, long.MaxValue)]
        public long Guid { get; set; }

        /// <summary>
        /// The post view count.
        /// </summary>
        public long ViewCount { get; set; }

        /// <summary>
        /// The post title.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The post text.
        /// </summary>
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// The url of the post image.
        /// </summary>
        //[Required]
        public string Image { get; set; }

        /// <summary>
        /// The summary of the post.
        /// </summary>
        [Required]
        public string Summary { get; set; }

        /// <summary>
        /// The date on which the post was posted.
        /// </summary>
        public DateTime DatePosted { get; set; }

        /// <summary>
        /// The collection of child comments of the post.
        /// </summary>
        public ICollection<Comment> Comments { get; set; }

        /// <summary>
        /// The property used for converting the <see cref="KeywordsList"/> to a string, and vice versa.
        /// </summary>
        [Required]
        public string Keywords
        {
            get { return KeywordsList == null ? "" : string.Join(",", KeywordsList); }
            set { KeywordsList = value?.Split(',').ToList(); }
        }

        /// <summary>
        /// The collection of the post keywords.
        /// </summary>
        [NotMapped]
        public ICollection<string> KeywordsList { get; set; }
    }
}
