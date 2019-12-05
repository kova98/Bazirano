using System.ComponentModel.DataAnnotations;

namespace Bazirano.Models.Identity
{
    /// <summary>
    /// The viewmodel class used for passing login information.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// The user's name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The user's password
        /// </summary>
        [Required]
        [UIHint("password")]
        public string Password { get; set; }

        /// <summary>
        /// The url to return to after login.
        /// </summary>
        public string ReturnUrl { get; set; }// = "/";
    }
}
