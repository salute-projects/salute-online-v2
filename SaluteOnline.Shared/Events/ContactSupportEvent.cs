using System;

namespace SaluteOnline.Shared.Events
{
    public class ContactSupportEvent
    {
        public string From { get; set; }
        public string Message { get; set; }
        public string SubjectId { get; set; }
        public long UserId { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
