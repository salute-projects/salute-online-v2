namespace SaluteOnline.Domain.DTO.Club
{
    public class ClubFilter : BaseFilter
    {
        public string Title { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public bool? IsFiim { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatorId { get; set; }
        public ClubStatus Status { get; set; }
    }
}
