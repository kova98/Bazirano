using System;
using System.ComponentModel.DataAnnotations;

namespace Bazirano.Models.Shared
{
    /// <summary>
    /// The comment model. Used for both news and threads. 
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// The id of the comment.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The comment author's username.
        /// </summary>
        [MaxLength(20, ErrorMessage = "Ime je predugačko.")]
        public string Username { get; set; }

        /// <summary>
        /// The date on which the comment was posted.
        /// </summary>
        public DateTime DatePosted { get; set; }

        /// <summary>
        /// The text of the comment.
        /// </summary>
        [Required(ErrorMessage = "Molimo unesite komentar.")]
        [MaxLength(1000, ErrorMessage = "Komentar je predugačak.")]
        [MinLength(10, ErrorMessage = "Komentar je prekratak.")]
        public string Text { get; set; }
    }
}
