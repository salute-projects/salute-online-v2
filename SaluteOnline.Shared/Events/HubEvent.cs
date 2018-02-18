using System;
using System.Collections.Generic;

namespace SaluteOnline.Shared.Events
{
    public class HubEvent
    {
        public Guid SenderGuid { get; set; }
        public List<Guid> Receivers { get; set; }
        public bool ToAll { get; set; }
        public string Method { get; set; }
        public dynamic Payload { get; set; }
    }
}
