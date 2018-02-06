using System.ComponentModel.DataAnnotations;

namespace SaluteOnline.IdentityServer.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
