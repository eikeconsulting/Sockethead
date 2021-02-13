using Sockethead.Razor.Helpers;
using System;
using System.Linq.Expressions;

namespace Sockethead.Razor.Grid
{
    public class ColumnBuilder<T> where T : class
    {
        private Column<T> Column { get; }

        internal ColumnBuilder(Column<T> column)
        {
            Column = column;
        }

        private ColumnBuilder<T> Wrap(Action action)
        {
            action();
            return this;
        }

        public ColumnBuilder<T> Header(string header) => Wrap(() => Column.HeaderValue = header);

        public ColumnBuilder<T> DisplayAs(Func<T, object> displayBuilder) => Wrap(() => Column.DisplayBuilder = displayBuilder);

        public ColumnBuilder<T> Encoded(bool isEncoded) => Wrap(() => Column.IsEncoded = isEncoded);

        public ColumnBuilder<T> LinkTo(Func<T, string> linkBuilder, string target = "_self")
            => Wrap(() =>
            {
                Column.LinkBuilder = linkBuilder;
                Column.LinkTarget = target;
            });

        public ColumnBuilder<T> Sortable(bool enable = true, SortOrder sortOrder = SortOrder.Ascending)
            => Wrap(() =>
            {
                Column.Sort.IsEnabled = enable;

                if (enable && Column.Expression == null)
                    throw new ArgumentException("You must pass an Expression into sort if not already specified.");

                if (!enable)
                    return;

                SortableBy(Column.Expression, sortOrder);
            });

        public ColumnBuilder<T> SortableBy(Expression<Func<T, object>> expression, SortOrder sortOrder = SortOrder.Ascending)
            => Wrap(() =>
            {
                Column.Sort.IsEnabled = true;
                Column.Sort.Expression = expression;
                Column.Sort.SortOrder = sortOrder;
            });

        public ColumnBuilder<T> For(Expression<Func<T, object>> expression)
            => Wrap(() =>
            {
                Column.Expression = expression;
                Header(expression.FriendlyName());
                Column.CompiledExpression = expression.Compile();
                DisplayAs(model => Column.CompiledExpression.Invoke(model));
            });

        public ColumnBuilder<T> AddHeaderCssClass(string cssClass) => Wrap(() => Column.HeaderDetails.CssClasses.Add(cssClass));

        public ColumnBuilder<T> AddHeaderCssStyle(string cssStyle) => Wrap(() => Column.HeaderDetails.CssStyles.Add(cssStyle));

        public ColumnBuilder<T> AddItemCssClass(string cssClass) => Wrap(() => Column.CssClasses.Add(cssClass));

        public ColumnBuilder<T> AddItemCssStyle(string cssStyle) => Wrap(() => Column.CssStyles.Add(cssStyle));
    }
}
