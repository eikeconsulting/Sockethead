using System.Collections.Generic;
using System.Linq;

namespace Sockethead.ExtensionsAndUtilities.Extensions
{
    /// <summary>
    /// Provides a set of extension methods that add additional functionality to collections
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns empty collection if source is null
        /// </summary>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}