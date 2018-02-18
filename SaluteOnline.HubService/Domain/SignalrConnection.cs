using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.HubService.Domain
{
    public class SignalrConnection : IMongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Guid { get; set; }

        [BsonElement("Id")]
        public int Id { get; set; }

        public string ConnectionId { get; set; }
        public bool Connected { get; set; }
    }
}
