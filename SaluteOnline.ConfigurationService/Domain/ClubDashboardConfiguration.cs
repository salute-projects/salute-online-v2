using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.ConfigurationService.Domain
{
    public class ClubDashboardConfiguration : IMongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Guid { get; set; }

        public int Id { get; set; }

        public DateTimeOffset LastChanged { get; set; }

        public int ClubId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserGuid { get; set; }

        public IEnumerable<ClubDashboardConfigurationItem> Panels { get; set; }
    }
}
