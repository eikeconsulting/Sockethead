using Microsoft.AspNetCore.Mvc.Rendering;
using Sockethead.Razor.Css;
using Sockethead.Razor.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Sockethead.Razor.Grid
{
    public class ColumnCssOptions
    {
        public CssBuilder Header { get; } = new CssBuilder();
        public CssBuilder Item { get; } = new CssBuilder();

        public void ClearAll()
        {
            Header.Clear();
            Item.Clear();
        }
    }

    public class ColumnBuilder<T> where T : class
    {
        private ColumnCssOptions CssOptions { get; } = new ColumnCssOptions();

        private Column<T> Column { get; }
        private IHtmlHelper Html { get; }

        internal ColumnBuilder(Column<T> column, IHtmlHelper html)
        {
            Column = column;
            Html = html;
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


        public ColumnBuilder<T> Css(Action<ColumnCssOptions> cssOptionsSetter)
        {
            cssOptionsSetter(CssOptions);

            Column.HeaderCss = CssOptions.Header.ToString();
            Column.ItemCss = CssOptions.Item.ToString();

            return this;
        }

        /// <summary>
        /// Embed a SimpleGrid inside this grid, how Meta!
        /// </summary>
        /// <typeparam name="TGrid">The entity type for the new SimpleGrid</typeparam>
        /// <param name="modelBuilder">Function to return the IQueryable model for the new SimpleGrid</param>
        /// <param name="builderAction">Action to build up the new SimpleGrid</param>
        public ColumnBuilder<T> SimpleGrid<TGrid>(
            Func<T, IQueryable<TGrid>> modelBuilder, 
            Action<SimpleGrid<TGrid>> builderAction) 
            where TGrid : class
            => Wrap(() =>
            {
                DisplayAs(model =>
                {
                    SimpleGrid<TGrid> grid = Html.SimpleGrid(modelBuilder(model));
                    builderAction.Invoke(grid);
                    return grid.RenderToString();
                })
                .Encoded(false);

            });

        /// <summary>
        /// Embed a TwoColumnGrid inside this SimpleGrid
        /// </summary>
        /// <param name="builderAction">An Action that takes the original model and a builder Action that 
        /// builds up the newly created TwoColumnGrid</param>
        public ColumnBuilder<T> TwoColumnGrid(Action<T, TwoColumnGridBuilder> builderAction)
            => Wrap(() =>
            {
                DisplayAs(model =>
                {
                    TwoColumnGridBuilder grid = Html.TwoColumnGrid();
                    builderAction.Invoke(model, grid);
                    return grid.RenderToString();
                })
                .Encoded(false);
            });

        /// <summary>
        /// Embed a TwoColumnGrid inside this SimpleGrid
        /// </summary>
        /// <typeparam name="TGrid">Type of the Model for the new TwoColumnGrid</typeparam>
        /// <param name="gridbuilder">Function to return the Model to use</param>
        public ColumnBuilder<T> TwoColumnGrid<TGrid>(Func<T, TGrid> gridbuilder)
            => Wrap(() =>
            {
                DisplayAs(model =>
                {
                    TwoColumnGridBuilder grid = Html.TwoColumnGrid();
                    grid.Add(gridbuilder(model));
                    return grid.RenderToString();
                })
                .Encoded(false);
            });

        /*
        public ColumnBuilder<T> AddHeaderCssClass(string cssClass) => Wrap(() => Column.HeaderDetails.CssClasses.Add(cssClass));

        public ColumnBuilder<T> AddHeaderCssStyle(string cssStyle) => Wrap(() => Column.HeaderDetails.CssStyles.Add(cssStyle));

        public ColumnBuilder<T> AddItemCssClass(string cssClass) => Wrap(() => Column.CssClasses.Add(cssClass));

        public ColumnBuilder<T> AddItemCssStyle(string cssStyle) => Wrap(() => Column.CssStyles.Add(cssStyle));
        */
    }
}
