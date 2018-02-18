using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.LogService.Domain
{
    public class Activity : IMongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Guid { get; set; }

        [BsonElement("Id")]
        public int Id { get; set; }

        public int UserId { get; set; }
        public ActivityType Type { get; set; }
        public ActivityImportance Importance { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Data { get; set; }
    }
}
