using Bazirano.Models.Shared;

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
        public long ArticleId { get; set; }

        /// <summary>
        /// The comment to post.
        /// </summary>
        public Comment Comment { get; set; }
    }
}
