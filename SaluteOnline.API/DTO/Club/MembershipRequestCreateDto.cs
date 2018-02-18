namespace SaluteOnline.API.DTO.Club
{
    public class MembershipRequestCreateDto
    {
        public int ClubId { get; set; }
        public string Nickname { get; set; }
        public bool SelectedFromExisting { get; set; }
    }
}
