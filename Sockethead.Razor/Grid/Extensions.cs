using Microsoft.AspNetCore.Html;
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

        /// <summary>
        /// Create an Ajax container for a PartialView containing a Grid
        /// The container will intercept Pagination clicks and dynamically load the grid via Ajax
        /// </summary>
        /// <param name="html">Html Helper</param>
        /// <param name="partialViewEndpoint">Endpoint to the Partial View containing the SimpleGrid</param>
        /// <param name="id">ID for the the container DIV.  This should be unique within your page.</param>
        /// <returns></returns>
        public static IHtmlContent SimpleGridAjax(this IHtmlHelper html, string partialViewEndpoint, string id, bool displaySearchField = false)
            => html.Partial(
                partialViewName: "_SHGridAjax", 
                model: new SimpleGridAjaxViewModel
                {
                    Endpoint = partialViewEndpoint,
                    Id = id,
                    DisplaySearchField = displaySearchField,
                });
    }
}
