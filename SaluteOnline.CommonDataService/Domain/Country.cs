using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.CommonDataService.Domain
{
    public class Country : IMongoEntity
    {
        [BsonIgnore]
        public Guid Guid { get; set; } = Guid.NewGuid();
        [BsonIgnore]
        public int Id { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("code")]
        public string Code { get; set; }
    }
}
