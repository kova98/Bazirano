using System;
using System.ComponentModel.DataAnnotations;

namespace Bazirano.Models.Shared
{
    public class Comment
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public DateTime DatePosted { get; set; }

        [Required(ErrorMessage = "Molimo unesite komentar.")]
        [MaxLength(1000, ErrorMessage = "Komentar je predugačak.")]
        [MinLength(10, ErrorMessage = "Komentar je prekratak.")]
        public string Text { get; set; }
    }
}
