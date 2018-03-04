using SaluteOnline.Shared.Common;

namespace SaluteOnline.Shared.Events
{
    public class UserRoleChangeEvent
    {
        public string SubjectId { get; set; }
        public Roles Role { get; set; }
    }
}
