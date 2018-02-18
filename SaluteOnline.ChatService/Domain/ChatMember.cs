using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.ChatService.Domain
{
    public class ChatMember : IMongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Guid { get; set; }
        public EntityType Type { get; set; }
        public DateTimeOffset Registered { get; set; }
        public string Title { get; set; }
        public int Id { get; set; }
    }
}
