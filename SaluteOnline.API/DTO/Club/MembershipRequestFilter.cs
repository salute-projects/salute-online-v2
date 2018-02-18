using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.DTO.Club
{
    public class MembershipRequestFilter : EntityFilter
    {
        public MembershipRequestStatus Status { get; set; }
    }
}
