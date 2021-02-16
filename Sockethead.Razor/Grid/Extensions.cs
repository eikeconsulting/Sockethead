using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Sockethead.Razor.Grid
{
    public static class Extensions
    {
        public static SimpleGrid<T> SimpleGrid<T>(this IHtmlHelper html, IQueryable<T> source) where T : class
                => new SimpleGrid<T>(html, source);

        public static SimpleGrid<T> SimpleGrid<T>(this IHtmlHelper html, IEnumerable<T> source) where T : class
                => new SimpleGrid<T>(html, source.AsQueryable());

        public static TwoColumnGridBuilder TwoColumnGrid(this IHtmlHelper html)
            => new TwoColumnGridBuilder(html);
    }
}
