using System;

namespace SaluteOnline.Domain.Extensions
{
    public static class MongoCoreExtensions
    {
        public static string ToMongoCollectionName(this Type entityType)
        {
            return entityType.Name.Replace("Mongo", string.Empty) + "s";
        }
    }
}
