using Sockethead.Razor.Helpers;
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

    public class Column<T> : SimpleGridBase, ISimpleGridColumn where T : class
    {
        internal Expression<Func<T, object>> Expression { get; set; }
        private Func<T, object> CompiledExpression { get; set; }
        internal Sort<T> Sort { get; set; } = new Sort<T>();
        private Func<T, object> DisplayBuilder { get; set; } = model => null;
        private Func<T, object> LinkBuilder { get; set; } = null;
        private string LinkTarget { get; set; }
        internal bool IsEncoded { get; set; } = true;
        private string HeaderValue { get; set; } = null;

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

        public Column<T> Header(string header) 
        { 
            HeaderValue = header; 
            return this; 
        }

        public Column<T> DisplayAs(Func<T, object> displayBuilder) 
        { 
            DisplayBuilder = displayBuilder; 
            return this; 
        }

        public Column<T> Encoded(bool isEncoded) 
        { 
            IsEncoded = isEncoded; 
            return this; 
        }

        public Column<T> LinkTo(Func<T, string> linkBuilder, string target = "_self") 
        { 
            LinkBuilder = linkBuilder; 
            LinkTarget = target; 
            return this; 
        }

        public Column<T> Sortable(bool enable = true, SortOrder sortOrder = SortOrder.Ascending)
        {
            Sort.IsEnabled = enable;

            if (enable && Expression == null)
                throw new ArgumentException("You must pass an Expression into sort if not already specified.");

            if (!enable)
                return this;

            return SortableBy(Expression, sortOrder);
        }

        public Column<T> SortableBy(Expression<Func<T, object>> expression, SortOrder sortOrder = SortOrder.Ascending)
        {
            Sort.IsEnabled = true;
            Sort.Expression = expression;
            Sort.SortOrder = sortOrder;
            return this;
        }

        public Column<T> For(Expression<Func<T, object>> expression)
        {
            Expression = expression;
            Header(expression.FriendlyName());
            CompiledExpression = expression.Compile();
            DisplayAs(model => CompiledExpression.Invoke(model));
            return this;
        }

        public Column<T> AddHeaderCssClass(string cssClass)
        {
            HeaderDetails.CssClasses.Add(cssClass);
            return this;
        }

        public Column<T> AddHeaderCssStyle(string cssStyle)
        {
            HeaderDetails.CssStyles.Add(cssStyle);
            return this;
        }

        public Column<T> AddItemCssClass(string cssClass)
        {
            CssClasses.Add(cssClass);
            return this;
        }

        public Column<T> AddItemCssStyle(string cssStyle)
        {
            CssStyles.Add(cssStyle);
            return this;
        }

    }
}
