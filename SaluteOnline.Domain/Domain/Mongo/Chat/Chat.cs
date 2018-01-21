using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaluteOnline.Domain.Common;

namespace SaluteOnline.Domain.Domain.Mongo.Chat
{
    public class Chat : IMongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Guid { get; set; }

        [BsonElement("Id")]
        public int Id { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

        public List<ChatMember> Participants { get; set; } = new List<ChatMember>();
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        public bool IsPrivate { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }
    }
}
