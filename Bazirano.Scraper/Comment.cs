using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bazirano.Scraper
{
    public class Comment
    {
        public long Id { get; set; }

        [MaxLength(20, ErrorMessage = "Ime je predugačko.")]
        public string Username { get; set; }

        public DateTime DatePosted { get; set; }

        [Required(ErrorMessage = "Molimo unesite komentar.")]
        [MaxLength(1000, ErrorMessage = "Komentar je predugačak.")]
        [MinLength(10, ErrorMessage = "Komentar je prekratak.")]
        public string Text { get; set; }

        public bool IsRoot { get; set; } = true;

        public List<Comment> Responses { get; set; }
    }
}
