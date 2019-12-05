using System;
using System.ComponentModel.DataAnnotations;

namespace Bazirano.Models.Board
{
    /// <summary>
    /// The board post model.
    /// </summary>
    public class BoardPost
    {
        /// <summary>
        /// The board post Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The board post image path.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The date on which the board post was posted.
        /// </summary>
        public DateTime DatePosted { get; set; }

        /// <summary>
        /// The board post text.
        /// </summary>
        [Required(ErrorMessage = "Molimo unesite tekst.")]
        [MaxLength(1000, ErrorMessage = "Tekst je predugačak.")]
        [MinLength(10, ErrorMessage = "Tekst je prekratak.")]
        public string Text { get; set; }
    }
}
