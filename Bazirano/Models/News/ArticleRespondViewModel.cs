using Bazirano.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace Bazirano.Models.News
{
    /// <summary>
    /// The viewmodel class used for responding to an article (a <see cref="NewsPost"/>).
    /// </summary>
    public class ArticleRespondViewModel
    {
        /// <summary>
        /// The id of the article to respond to.
        /// </summary>
        [Required]
        [Range(1, long.MaxValue)]
        public long ArticleId { get; set; }

        /// <summary>
        /// The comment to post.
        /// </summary>
        [Required]
        public Comment Comment { get; set; }
    }
}
