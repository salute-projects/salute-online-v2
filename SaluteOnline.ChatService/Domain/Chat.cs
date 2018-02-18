using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.ChatService.Domain
{
    public class Chat : IMongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Guid { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

        public List<Guid> Participants { get; set; } = new List<Guid>();
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        public bool IsPrivate { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }
        public int Id { get; set; }
    }
}
