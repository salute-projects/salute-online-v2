using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SaluteOnline.Shared.Extensions
{
    public static class QueryableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Expression<Func<TSource, TKey>> key)
        {
            return source.AsQueryable().GroupBy(key).Select(t => t.FirstOrDefault());
        } 
    }
}
