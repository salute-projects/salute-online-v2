using System;

namespace SaluteOnline.Shared.Extensions
{
    public static class MongoCoreExtensions
    {
        public static string ToMongoCollectionName(this Type entityType)
        {
            return entityType.Name.Replace("Mongo", string.Empty) + "s";
        }
    }
}
