using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Sockethead.Common.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Conditionally do the transformation. See `WhereIf` below to
        /// see how it is used for conditionally include `Where`
        /// </summary>
        public static IQueryable<E> If<E>(
            this IQueryable<E> source,
            bool condition,
            Func<IQueryable<E>, IQueryable<E>> transform
        ) => condition ? transform(source) : source;

        /// <summary>
        /// Include `predicate` if `condition` is true
        /// </summary>
        public static IQueryable<E> WhereIf<E>(
            this IQueryable<E> source,
            bool condition,
            Expression<Func<E, bool>> predicate
        ) => source.If(condition, s => s.Where(predicate));

        /// <summary>
        /// Enables pagination of a queryable source by returning a specific number of elements based on a zero-indexed page number and a specified page size.
        /// </summary>
        public static IQueryable<E> Paginate<E>(this IQueryable<E> source, int pageNumber, int pageSize)
            => source.Skip(pageSize * pageNumber).Take(pageSize);

        /// <summary>
        /// Applies the IgnoreQueryFilters feature if the provided condition is true, otherwise returns the original source.
        /// </summary>
        public static IQueryable<TEntity> IgnoreQueryFiltersIf<TEntity>(
            this IQueryable<TEntity> source,
            bool condition
        ) where TEntity : class => condition ? source.IgnoreQueryFilters() : source;
    }
}