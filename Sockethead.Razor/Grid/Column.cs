using System;
using System.Linq.Expressions;
using System.Web;

namespace Sockethead.Razor.Grid
{
    public interface ISimpleGridColumn
    {
        string Css();
        string DisplayRender(object model);

        SimpleGridColumnHeader HeaderDetails { get; }
    }

    public class SimpleGridColumnHeader : SimpleGridBase
    {
        public string Display { get; set; }
        public string SortUrl { get; set; }
        public SortOrder CurrentSortOrder { get; set; }
    }

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

    internal class Column<T> : SimpleGridBase, ISimpleGridColumn where T : class
    {
        internal Expression<Func<T, object>> Expression { get; set; }
        internal Func<T, object> CompiledExpression { get; set; }
        internal Sort<T> Sort { get; set; } = new Sort<T>();
        internal Func<T, object> DisplayBuilder { get; set; } = model => null;
        internal Func<T, object> LinkBuilder { get; set; } = null;
        internal string LinkTarget { get; set; }
        internal bool IsEncoded { get; set; } = true;
        internal string HeaderValue { get; set; } = null;


        public SimpleGridColumnHeader HeaderDetails { get; } = new SimpleGridColumnHeader();

        internal string HeaderRender() => HttpUtility.HtmlEncode(HeaderValue);

        public string DisplayRender(object model)
        {
            object result = DisplayBuilder((T)model);
            string display = result == null ? "" : result.ToString();

            if (IsEncoded)
                display = HttpUtility.HtmlEncode(display);

            if (LinkBuilder != null)
                display = $"<a href='{LinkBuilder.Invoke((T)model)}' target='{LinkTarget}'>{display}</a>";

            return display;
        }
    }
}
