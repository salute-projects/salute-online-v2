namespace SaluteOnline.API.DTO.Club
{
    public class ClubSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public bool CanBeEdited { get; set; }
    }
}
