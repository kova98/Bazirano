using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.AuthorInterface
{
    public class ChangePasswordViewModel
    {
        public string CurrentPassword { get; set; } = "";

        public string NewPassword { get; set; } = "";

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
