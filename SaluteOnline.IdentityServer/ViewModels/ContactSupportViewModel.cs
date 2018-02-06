using System.ComponentModel.DataAnnotations;
using SaluteOnline.IdentityServer.Domain;

namespace SaluteOnline.IdentityServer.ViewModels
{
    public class ContactSupportViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public ErrorType Type { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
