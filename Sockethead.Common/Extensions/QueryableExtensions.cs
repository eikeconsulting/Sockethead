using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
        
        /// <summary>
        /// Takes a collection and breaks it into "chunks" according to chunkSize
        /// and passes each chunk on to the delegate action for processing.
        /// </summary>
        /// <param name="collection">original collection to chunk</param>
        /// <param name="chunkSize">size of each chunk</param>
        /// <param name="action">delegate function to process each chunk</param>
        private static void ForEachChunk<T>(
            this IQueryable<T> collection,
            int chunkSize,
            Action<List<T>> action)
        {
            for (int chunk = 0; ; chunk++)
            {
                List<T> currentChunk = collection
                   .Skip(chunk * chunkSize)
                   .Take(chunkSize)
                   .ToList();

                if (!currentChunk.Any())
                    break;

                action.Invoke(currentChunk);
            }
        }

        /// <summary>
        /// Takes a collection and breaks it into "chunks" according to chunkSize
        /// and passes each chunk on to the delegate action for processing.
        /// </summary>
        /// <param name="collection">original collection to chunk</param>
        /// <param name="chunkSize">size of each chunk</param>
        /// <param name="action">delegate function to process each chunk</param>
        /// <param name="cancellationToken">token to stop processing</param>
        private static async Task ForEachChunkAsync<T>(
            this IQueryable<T> collection,
            int chunkSize,
            Action<List<T>> action,
            CancellationToken cancellationToken = default)
        {
            for (int chunk = 0; ; chunk++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                List<T> currentChunk = await collection
                   .Skip(chunk * chunkSize)
                   .Take(chunkSize)
                   .ToListAsync(cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                    break;

                if (!currentChunk.Any())
                    break;

                action.Invoke(currentChunk);
            }
        }

        /// <summary>
        /// This is equivalent to the standard ForEach extension, but divides the
        /// original collection into chunks rather than pulling it all in one query.
        /// This uses ForEachChunk helper to break it into chunks.
        /// </summary>
        /// <typeparam name="T">collection entity type</typeparam>
        /// <param name="collection">collection to process (this)</param>
        /// <param name="chunkSize">size of the chunks to process at one time</param>
        /// <param name="action">delegate function to process each individual entity</param>
        public static void ForEachInChunks<T>(this IQueryable<T> collection, int chunkSize, Action<T> action)
        {
            collection.ForEachChunk(chunkSize, chunk => chunk.ForEach(action.Invoke));
        }

        public static async Task ForEachInChunksAsync<T>(
            this IQueryable<T> collection,
            int chunkSize,
            Action<T> action,
            CancellationToken cancellationToken = default)
        {
            await collection.ForEachChunkAsync(chunkSize, chunk => chunk.ForEach(action.Invoke), cancellationToken);
        }


        /// <summary>
        /// Breaks the original collection into smaller "chunks" based on a specified chunk size, and return these chunks
        /// as a sequence of lists. This method allows for asynchronous processing of large collections, where pulling all
        /// the data into memory at once may not be feasible or efficient.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="chunkSize"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async IAsyncEnumerable<List<T>> ChunkAsync<T>(
            this IQueryable<T> collection,
            int chunkSize,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            for (int pos = 0; !cancellationToken.IsCancellationRequested; pos += chunkSize)
            {
                List<T> result = await collection
                    .Skip(pos)
                    .Take(chunkSize)
                    .ToListAsync(cancellationToken);

                if (!result.Any())
                    break;

                yield return result;
            }
        }


        /// <summary>
        /// This allows you to loop over a list of items that shrinks the original query as you process it
        /// It will terminate in one of two cases:
        /// 1. The query returns zero results
        /// 2. The query returns the same or more results than the previous time through the loop
        /// </summary>
        public static async Task ForEachInChunksForShrinkingListAsync<T>(
            this IQueryable<T> collection,
            int chunkSize,
            Func<T, Task> action,
            Func<Task> onChunkComplete)
        {
            for (int last = int.MaxValue; ;)
            {
                // how many records are remaining to process?
                int curr = await collection.CountAsync();
                if (curr == 0)
                    break;

                // after each process, the remaining record count should go down
                // this avoids and infinite loop in case there is an problem processing
                // basically, we bail if no progress is made at all
                if (last <= curr)
                    return;

                last = curr;

                foreach (T item in await collection.Take(chunkSize).ToListAsync())
                    await action(item);

                await onChunkComplete();
            }
        }

        /// <summary>
        /// Process a Queryable whose result set changes as you process each chunk
        /// </summary>
        /// <param name="query">Queryable that changes</param>
        /// <param name="maxRecords">Maximum records to process in total</param>
        /// <param name="chunkSize">Size of chunk to process in memory at one time</param>
        /// <param name="onProcessItem">Item processing callback</param>
        /// <param name="onChunkComplete">Callback to invoke after each chunk is complete</param>
        /// <returns>Total number of records processed</returns>
        public static async Task<int> ShrinkingListChunker<T>(
            this IQueryable<T> query,
            int maxRecords,
            int chunkSize,
            Func<T, Task> onProcessItem,
            Func<Task> onChunkComplete)
        {
            int taken = 0;

            while (taken < maxRecords)
            {
                int take = Math.Min(chunkSize, maxRecords - taken);

                List<T> source = await query.Take(take).ToListAsync();

                if (!source.Any())
                    break;

                foreach (T item in source)
                    await onProcessItem(item);

                await onChunkComplete();

                taken += source.Count;
            }

            return taken;
        }
    }
}