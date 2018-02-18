using System;

namespace SaluteOnline.API.DTO.Club
{
    public class ClubMemberSummary
    {
        public int? PlayerId { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; }
        public string Nickname { get; set; }
        public DateTimeOffset Registered { get; set; }
        public string Avatar { get; set; }
    }
}
