using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.AuthorInterface
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Ovo polje je obavezno.")]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage = "Ovo polje je obavezno.")]
        public string NewPassword { get; set; } = "";

        [Compare(nameof(NewPassword), ErrorMessage = "Lozinke se ne poklapaju")]
        public string ConfirmPassword { get; set; } = "";

        public ChangePasswordViewModel() { }

        public ChangePasswordViewModel(string currentPassword, string newPassword, string confirmPassword)
        {
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
            ConfirmPassword = confirmPassword;
        }
    }
}
