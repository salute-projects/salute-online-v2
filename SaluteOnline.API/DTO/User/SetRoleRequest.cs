using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.DTO.User
{
    public class SetRoleRequest
    {
        public int UserId { get; set; }
        public Roles Role { get; set; }
    }
}
