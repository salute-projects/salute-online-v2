namespace SaluteOnline.Shared.Events
{
    public class UserCreatedEvent
    {
        public string SubjectId { get; set; }
        public long UserId { get; set; }
    }
}
