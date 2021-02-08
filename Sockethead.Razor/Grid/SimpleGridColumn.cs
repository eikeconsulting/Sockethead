using Sockethead.Razor.Helpers;
using System;
using System.Linq.Expressions;
using System.Web;

namespace Sockethead.Razor.Grid
{
    public class SimpleGridColumn<T> : SimpleGridBase where T : class
    {
        private Expression<Func<T, string>> Expression { get; set; }
        private Func<T, string> CompiledExpression { get; set; }
        internal SimpleGridSort<T> Sort { get; set; } = new SimpleGridSort<T>();
        internal bool IsSortable => Sort.Expression != null;
        internal string LabelValue { get; set; } = null;
        internal Func<T, string> DisplayBuilder { get; set; } = model => null;
        private Func<T, string> LinkBuilder { get; set; } = null;
        private string LinkTarget { get; set; }
        internal bool IsEncoded { get; set; } = true;

        internal string LabelRender() => HttpUtility.HtmlEncode(LabelValue);

        internal string DisplayRender(T model)
        {
            string display = DisplayBuilder(model);

            if (IsEncoded)
                display = HttpUtility.HtmlEncode(display);

            if (LinkBuilder != null)
                display = $"<a href='{LinkBuilder.Invoke(model)}' target='{LinkTarget}'>{display}</a>";

            return display;
        }

        public SimpleGridColumn<T> Label(string label) { LabelValue = label; return this; }

        public SimpleGridColumn<T> DisplayAs(Func<T, string> displayBuilder) { DisplayBuilder = displayBuilder; return this; }

        public SimpleGridColumn<T> Encoded(bool isEncoded) { IsEncoded = isEncoded; return this; }

        public SimpleGridColumn<T> LinkTo(Func<T, string> linkBuilder, string target = "_self") 
        { 
            LinkBuilder = linkBuilder; 
            LinkTarget = target; 
            return this; 
        }

        public SimpleGridColumn<T> Sortable(bool enable = true, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (enable && Expression == null)
                throw new ArgumentException("You must pass an Expression into sort if not already specified");

            if (!enable)
            {
                Sort.Expression = null;
                return this;
            }

            return SortableAs(Expression, sortOrder);
        }

        public SimpleGridColumn<T> SortableAs(Expression<Func<T, string>> expression, SortOrder sortOrder = SortOrder.Ascending)
        {
            Sort.Expression = expression;
            Sort.SortOrder = sortOrder;
            return this;
        }

        public SimpleGridColumn<T> For(Expression<Func<T, string>> expression)
        {
            Expression = expression;
            Label(expression.FriendlyName());
            CompiledExpression = expression.Compile();
            DisplayAs(model => CompiledExpression.Invoke(model));
            return this;
        }

        public SimpleGridColumn<T> AddCssClass(string cssClass)
        {
            CssClasses.Add(cssClass);
            return this;
        }

        public SimpleGridColumn<T> AddCssStyle(string cssStyle)
        {
            CssStyles.Add(cssStyle);
            return this;
        }
    }
}
