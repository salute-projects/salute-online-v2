namespace SaluteOnline.Domain.DTO.Club
{
    public class HandleMembershipRequestDto
    {
        public int ClubId { get; set; }
        public int RequestId { get; set; }
        public MembershipRequestStatus Status { get; set; }
    }
}
