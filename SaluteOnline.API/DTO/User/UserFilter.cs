using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.DTO.User
{
    public class UserFilter : BaseFilter
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public Roles Role { get; set; }
        public UserStatus Status { get; set; }
    }
}
