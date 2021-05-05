using System;
using System.Linq.Expressions;
using System.Web;

namespace Sockethead.Razor.Grid
{
    public interface ISimpleGridColumn
    {
        string HeaderCss { get; }
        string ItemCss { get; }
        string DisplayRender(object model);
        ColumnHeader HeaderDetails { get; }
    }

    public class ColumnHeader
    {
        public string Display { get; set; }
        public string SortUrl { get; set; }
        public SortOrder CurrentSortOrder { get; set; }
    }

    internal class Column<T> : ISimpleGridColumn where T : class
    {
        //internal Expression<Func<T, object>> Expression { get; set; }
        internal Func<T, object> CompiledExpression { get; set; }
        internal Sort<T> Sort { get; set; } = new Sort<T>();
        internal Func<T, object> DisplayBuilder { get; set; } = model => null;
        internal Func<T, object> LinkBuilder { get; set; } = null;
        internal string LinkTarget { get; set; }
        internal Css.CssBuilder LinkCssBuilder { get; set; }
        internal bool IsEncoded { get; set; } = true;
        internal string HeaderValue { get; set; } = null;
        internal int Order { get; set; } = int.MaxValue;

        public string HeaderCss { get; set; }
        public string ItemCss { get; set; }

        public ColumnHeader HeaderDetails { get; } = new ColumnHeader();

        internal string HeaderRender() => HttpUtility.HtmlEncode(HeaderValue);

        public string DisplayRender(object model)
        {
            object result = DisplayBuilder((T)model);
            string display = result == null ? "" : result.ToString();

            if (IsEncoded)
                display = HttpUtility.HtmlEncode(display);

            if (LinkBuilder != null)
                display = $"<a {LinkCssBuilder} href='{LinkBuilder.Invoke((T)model)}' target='{LinkTarget}'>{display}</a>";

            return display;
        }
    }
}


/*
/// <summary>
/// TODO - not used yet...
/// </summary>
public class ColumnViewModel
{
    private ISimpleGridColumn Column { get; }

    public ColumnViewModel(ISimpleGridColumn column)
    {
        Column = column;
    }

    public string Css => Column.Css();
    public string HeaderDisplay => Column.HeaderDetails.Display;
    public string HeaderSortUrl => Column.HeaderDetails.SortUrl;
    public SortOrder HeaderSortOrder => Column.HeaderDetails.CurrentSortOrder;
    public string Display(object model) => Column.DisplayRender(model);
}
*/