using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaluteOnline.Domain.Common;
using SaluteOnline.Domain.DTO;

namespace SaluteOnline.Domain.Domain.Mongo
{
    public class Activity : IMongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Guid { get; set; }

        [BsonElement("Id")]
        public int Id { get; set; }

        public int UserId { get; set; }
        public Enums.ActivityType Type { get; set; }
        public Enums.ActivityImportance Importance { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Data { get; set; }
    }
}
