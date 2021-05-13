using Sockethead.Razor.Grid;
using System;

namespace Sockethead.Web.Extensions
{
    public static class SampleSimpleGridExtensions
    {
        public static SimpleGrid<T> MyFavoriteGridStuff<T>(
            this SimpleGrid<T> grid) where T : class 
            => grid
                .Css(elements => elements.Table.AddClass("table-striped"))
                .AddPager(options =>
                {
                    options.TopAndBottom();
                    options.RowsPerPage = 5;
                    options.RowsPerPageOptions = new[] { 5, 10, 25 };
                });

        public static ColumnBuilder<T> MyGoogleSearchButton<T>(
            this ColumnBuilder<T> col,
            string text,
            Func<T, string> queryBuilder) where T : class 
            => col
                .Display(text)
                .LinkTo(
                    linkBuilder: model =>
                    {
                        string query = queryBuilder(model);
                        string encoded = System.Web.HttpUtility.UrlEncode(query);
                        return $"https://www.google.com/search?q={encoded}";
                    }, 
                    target: "_blank",
                    css: css => css.AddClass("btn btn-primary"));

    }
}
