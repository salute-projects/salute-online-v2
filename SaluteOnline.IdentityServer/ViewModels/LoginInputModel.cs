using System.ComponentModel.DataAnnotations;

namespace SaluteOnline.IdentityServer.ViewModels
{
    public class LoginInputModel : BaseViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
    }
}
