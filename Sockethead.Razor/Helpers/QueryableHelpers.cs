using System;
using System.Collections.Generic;
using System.Linq;

namespace Sockethead.Razor.Helpers
{
    public static class QueryableHelpers
    {
        public static IQueryable<T> NoResults<T>(this IQueryable<T> source) where T : class
            => source.Where(s => false);

        public static bool TryFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource result)
            => (result = source.FirstOrDefault(predicate)) != null;
    }
}
