using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.DTO.User
{
    public class SetStatusRequest
    {
        public int UserId { get; set; }
        public UserStatus Status { get; set; }
    }
}
