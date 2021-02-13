using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sockethead.Razor.Grid
{
    public enum SortOrder
    { 
        None,
        Ascending,
        Descending,
    }

    internal class Sort<T> where T : class
    {
        public Expression<Func<T, object>> Expression { get; set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsActive => IsEnabled && Expression != null;

        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

        public SortOrder SortOrderFlipped => Flip(SortOrder);

        public static SortOrder Flip(SortOrder sortOrder)
            => sortOrder switch
            {
                SortOrder.Ascending => SortOrder.Descending,
                SortOrder.Descending => SortOrder.Ascending,
                _ => SortOrder.Ascending,
            };

        public Sort<T> Flip()
        {
            SortOrder = SortOrderFlipped;
            return this;
        }

        public IQueryable<T> ApplyTo(IQueryable<T> query, bool isThenBy)
        {
            if (Expression == null)
                return query;

            return isThenBy && query is IOrderedQueryable<T> orderedSource
                ? SortOrder switch
                {
                    SortOrder.Ascending => orderedSource.ThenBy(Expression),
                    SortOrder.Descending => orderedSource.ThenByDescending(Expression),
                    _ => orderedSource.ThenBy(Expression),
                }
                :
                SortOrder switch
                {
                    SortOrder.Ascending => query.OrderBy(Expression),
                    SortOrder.Descending => query.OrderByDescending(Expression),
                    _ => query.OrderBy(Expression),
                };
        }

        public static IQueryable<T> ApplySorts(IEnumerable<Sort<T>> sorts, IQueryable<T> query)
        {
            int i = 0;
            foreach (var sort in sorts)
                query = sort.ApplyTo(query, isThenBy: i++ > 0);
            return query;
        }
    }
}
