using System;
using SaluteOnline.Domain.Common;
using SaluteOnline.Domain.DTO;

namespace SaluteOnline.Domain.Domain.EF
{
    public class InnerMessage : IEntity
    {
        public Guid Guid { get; set; }
        public int Id { get; set; }

        public int? SenderId { get; set; }
        public EntityType SenderType { get; set; }

        public int? ReceiverId { get; set; }
        public EntityType ReceiverType { get; set; }

        public MessageStatus Status { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastActivity { get; set; }

        public bool SentBySystem { get; set; }
        public bool OneResponseForAll { get; set; }
    }
}
