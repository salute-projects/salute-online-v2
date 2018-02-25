using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.DTO.Club
{
    public class ClubChangeStatusRequest
    {
        public int ClubId { get; set; }
        public ClubStatus Status { get; set; }
    }
}
