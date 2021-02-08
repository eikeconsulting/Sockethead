using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sockethead.Razor.Grid
{
    public class SimpleGridBase
    {
        public List<string> CssClasses { get; } = new List<string>();
        public List<string> CssStyles { get; } = new List<string>();

        public string Css() 
        {
            var sb = new StringBuilder();

            if (CssClasses.Any())
                sb.Append($" class='{string.Join(" ", CssClasses)}'");

            if (CssStyles.Any())
                sb.Append($" style='{string.Join(";", CssStyles)}'");

            return sb.ToString();
        }
    }
}

/*
public static IHtmlGrid<T> Pageable<T>(this IHtmlGrid<T> html, IQueryable<T> model, int rowsPerPage = 25)
   => model.Count() <= rowsPerPage
        ? html
        : html
            .Pageable(pager =>
            {
                pager.PageSizes = new Dictionary<int, string>
                    {
                        { 10, "10" },
                        { 25, "25" },
                        { 100, "100" },
                        { 1000, "1,000" },
                        { 5000, "5,000" },
                    };
                pager.ShowPageSizes = true;
                pager.RowsPerPage = rowsPerPage;
            });
*/