namespace SaluteOnline.Shared.Events
{
    public class UserRegisteredEvent
    {
        public string SubjectId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
