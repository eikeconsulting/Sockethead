using Microsoft.AspNetCore.Mvc.Rendering;
using Sockethead.Razor.Css;
using Sockethead.Razor.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;
using System.Collections.Generic;

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

        /// <summary>
        /// Generate the Header from an Expression
        /// </summary>
        public ColumnBuilder<T> HeaderFor(Expression<Func<T, object>> expression) => Header(expression.FriendlyName());

        /// <summary>
        /// Specify the Header for the Column.
        /// The Header is also set by the "For(model => model.MyProp)" as well, 
        /// so this isn't necessary in most cases
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public ColumnBuilder<T> Header(string header) { Column.HeaderValue = header; return this; }

        /// <summary>
        /// What to render
        /// </summary>
        public ColumnBuilder<T> DisplayAs(Func<T, object> displayBuilder)
        {
            Column.DisplayFunc = displayBuilder;
            return this;
        }

        /// <summary>
        /// Display a static value for this column for all rows 
        /// </summary>
        public ColumnBuilder<T> Display(string value) 
            => DisplayAs(model => value);

        public ColumnBuilder<T> DisplayExpression(Expression<Func<T, object>> expression)
        {
            Func<T, object> compiled = (Column.DisplayExpr = expression).Compile();

            return ExpressionHelpers
                .GetObjectType(expression)
                .IsSubclassOf(typeof(Enum))
                    ? DisplayAs(model => (compiled(model) as Enum).GetDisplayName())
                    : DisplayAs(compiled);
        }

        public ColumnBuilder<T> DisplayAsList(Func<T, IEnumerable<object>> displayBuilder)
            => DisplayAs(model => HtmlUtilities.BuildList(displayBuilder(model))).Encoded(false);

        /// <summary>
        /// Display IHtmlContent in the column.
        /// This can be used in conjunction with Html.DisplayFor (see example)
        /// </summary>
        public ColumnBuilder<T> DisplayHtmlContent(Func<T, IHtmlContent> htmlBuilder) 
            => DisplayAs(model =>
            {
                var content = htmlBuilder(model);
                var stringWriter = new System.IO.StringWriter();
                content.WriteTo(stringWriter, HtmlEncoder.Default);
                return stringWriter.ToString();
            })
            .Encoded(false);

        /// <summary>
        /// Html encode
        /// </summary>
        public ColumnBuilder<T> Encoded(bool isEncoded) 
        { 
            Column.IsEncoded = isEncoded; 
            return this; 
        }

        /// <summary>
        /// Create a link
        /// </summary>
        /// <param name="linkBuilder">Build the destination URL</param>
        /// <param name="target">target (e.g. _self, or _blank)</param>
        /// <param name="css">CSS to apply to the link</param>
        /// <returns></returns>
        public ColumnBuilder<T> LinkTo(Func<T, string> linkBuilder, string target = "_self", Action<CssBuilder> css = null)
        {
            Column.LinkBuilder = linkBuilder;
            Column.LinkTarget = target;
            var cssBuilder = new CssBuilder();
            css?.Invoke(cssBuilder);
            Column.LinkCssBuilder = cssBuilder;
            return this;
        }

        /// <summary>
        /// Make the entire table sortable by those columns that are Sortable
        /// Individual Columns are sortable if they have an Expression and aren't otherwise sort disabled
        /// </summary>
        public ColumnBuilder<T> Sortable(bool enable = true, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (enable && Column.Sort.Expression == null)
                throw new ArgumentException("You must call For() or SortableBy() with a valid Expression to enable Sorting on this Column.");

            Column.Sort.IsEnabled = enable;
            Column.Sort.SortOrder = sortOrder;
            return this;
        }

        /// <summary>
        /// Specify the expression to sort the column by
        /// By default calling "For(model => model.MyProp)" sets the Sort Expression as well, 
        /// so in most cases this isn't needed.
        /// </summary>
        public ColumnBuilder<T> SortableBy(Expression<Func<T, object>> expression, SortOrder sortOrder = SortOrder.Ascending)
        {
            Column.Sort.IsEnabled = true;
            Column.Sort.Expression = expression;
            Column.Sort.SortOrder = sortOrder;
            return this;
        }

        /// <summary>
        /// Specify an Expression for the column which initializes the Column:
        /// 1. Header (via the Expression)
        /// 2. Sort Expression
        /// 3. DisplayAs Expression
        /// </summary>
        public ColumnBuilder<T> For(Expression<Func<T, object>> expression)
        {
            HeaderFor(expression);
            SortableBy(expression);
            DisplayExpression(expression);
            return this;
        }

        /// <summary>
        /// Specify CSS options for the Grid
        /// </summary>
        public ColumnBuilder<T> Css(Action<ColumnCssOptions> cssOptionsSetter)
        {
            cssOptionsSetter(CssOptions);
            Column.HeaderCss = CssOptions.Header.ToString();
            Column.ItemCss = CssOptions.Item.ToString();
            return this;
        }

        /// <summary>
        /// Perform column building operations only if a condition is met
        /// </summary>
        public ColumnBuilder<T> If(bool condition, Action<ColumnBuilder<T>> conditionalAction)
        {
            if (condition)
                conditionalAction(this);
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
            => DisplayAs(model =>
            {
                SimpleGrid<TGrid> grid = Html.SimpleGrid(modelBuilder(model));
                builderAction.Invoke(grid);
                return grid.RenderToString();
            })
            .Encoded(false);

        /// <summary>
        /// Embed and build a TwoColumnGrid inside this SimpleGrid
        /// </summary>
        /// <param name="builderAction">An Action that takes the original model and a builder Action that 
        /// builds up the newly created TwoColumnGrid</param>
        public ColumnBuilder<T> TwoColumnGrid(Action<T, TwoColumnGridBuilder> builderAction)
            => DisplayAs(model =>
            {
                TwoColumnGridBuilder grid = Html.TwoColumnGrid();
                builderAction.Invoke(model, grid);
                return grid.RenderToString();
            })
            .Encoded(false);

        /// <summary>
        /// Embed a TwoColumnGrid inside this SimpleGrid from a Model
        /// </summary>
        /// <typeparam name="TGrid">Type of the Model for the new TwoColumnGrid</typeparam>
        /// <param name="gridModelBuilder">Function to return the Model to use</param>
        public ColumnBuilder<T> TwoColumnGrid<TGrid>(Func<T, TGrid> gridModelBuilder)
            => TwoColumnGrid((model, grid) => grid.AddRowsForModel(gridModelBuilder(model)));

        /// <summary>
        /// Render a DateTime as a time html tag to be processed on the client side
        /// </summary>
        public ColumnBuilder<T> ClientTime(Func<T, DateTime?> timeBuilder)
            => DisplayAs(model => Html.ClientTime(timeBuilder(model)))
            .Encoded(false);
    }
}
