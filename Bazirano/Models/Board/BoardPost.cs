using System;
using System.ComponentModel.DataAnnotations;

namespace Bazirano.Models.Board
{
    public class BoardPost
    {
        public long Id { get; set; }
        public string Image { get; set; }
        public DateTime DatePosted { get; set; }

        [Required(ErrorMessage = "Molimo unesite tekst.")]
        [MaxLength(500, ErrorMessage = "Tekst je predugačak.")]
        [MinLength(10, ErrorMessage = "Tekst je prekratak.")]
        public string Text { get; set; }
    }
}
