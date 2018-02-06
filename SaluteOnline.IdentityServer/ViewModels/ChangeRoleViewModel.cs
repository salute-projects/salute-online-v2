using SaluteOnline.Domain.DTO;

namespace SaluteOnline.IdentityServer.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string Email { get; set; }
        public Roles NewRole { get; set; }
    }
}
