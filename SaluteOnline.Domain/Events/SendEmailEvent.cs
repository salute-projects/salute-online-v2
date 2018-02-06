using System.Collections.Generic;

namespace SaluteOnline.Domain.Events
{
    public class SendEmailEvent
    {
        public string From { get; set; }
        public List<string> To { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string TextBody { get; set; }
        public List<string> Cc { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
        public string ReplyTo { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
