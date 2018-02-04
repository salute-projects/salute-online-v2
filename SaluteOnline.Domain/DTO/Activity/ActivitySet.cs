namespace SaluteOnline.Domain.DTO.Activity
{
    public class ActivitySet
    {
        public int UserId { get; set; }
        public ActivityType Type { get; set; }
        public ActivityImportance Importance { get; set; }
        public string Data { get; set; }
    }
}
