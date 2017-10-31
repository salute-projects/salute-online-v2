using System;

namespace SaluteOnline.Domain.DTO.Club
{
    public class ClubDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Registered { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
        public bool IsFiim { get; set; }
        public bool IsActive { get; set; }
        public int CreatorId { get; set; }
        public ClubStatus Status { get; set; }
        public string Logo { get; set; }
    }
}
