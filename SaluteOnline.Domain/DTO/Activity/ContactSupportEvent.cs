using System;

namespace SaluteOnline.Domain.DTO.Activity
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
