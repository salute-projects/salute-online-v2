using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaluteOnline.Domain.Common;

namespace SaluteOnline.Domain.Domain.Mongo
{
    public class ConnectedUser : IMongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Guid { get; set; }
        [BsonElement("Id")]
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTimeOffset Connected { get; set; }
        public List<SignalrConnection> Connections { get; set; } = new List<SignalrConnection>();
    }
}
