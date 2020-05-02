using System.ComponentModel.DataAnnotations;

namespace Bazirano.Models.Board
{
    public class SubmitViewModel
    {
        [Required(ErrorMessage = "Molimo unesite tekst.")]
        [MaxLength(1000, ErrorMessage = "Tekst je predugačak.")]
        [MinLength(10, ErrorMessage = "Tekst je prekratak.")]
        public string Text { get; set; }

        [Url(ErrorMessage = "Molimo unesite poveznicu na sliku")]
        public string ImageUrl { get; set; }
    }
}
