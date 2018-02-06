using System.ComponentModel.DataAnnotations;

namespace SaluteOnline.IdentityServer.ViewModels
{
    public class RegisterUserViewModel : BaseViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password must be at least 6 characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation does not match.")]
        public string ConfirmPassword { get; set; }

        [StringLength(50)]
        [Display(Name = "Nickname")]
        public string Nickname { get; set; }
    }
}
