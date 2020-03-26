using System.ComponentModel.DataAnnotations;

namespace Bazirano.Models.Column
{
    public class Author
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Ovo polje je obavezno.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Ovo polje je obavezno.")]
        public string Bio { get; set; }

        [Required(ErrorMessage = "Ovo polje je obavezno.")]
        public string ShortBio { get; set; }

        [Required(ErrorMessage = "Ovo polje je obavezno.")]
        public string Image { get; set; }
    }
}
