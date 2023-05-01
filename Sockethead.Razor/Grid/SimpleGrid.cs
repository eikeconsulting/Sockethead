using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sockethead.Razor.Css;
using Sockethead.Razor.Helpers;
using Sockethead.Razor.Pager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Sockethead.Razor.Grid
{
    public partial class SimpleGrid<T> where T : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="html">HTML Helper</param>
        /// <param name="source">Data Source</param>
        public SimpleGrid(IHtmlHelper html, IQueryable<T> source, string gridId = null)
        {
            Html = html;
            Source = source;
            GridId = gridId;
            State = new State(Html.ViewContext.HttpContext.Request, gridId);
        }

        private IHtmlHelper Html { get; }
        private IQueryable<T> Source { get; }
        private string GridId { get; }
        private State State { get; }
        private SimpleGridOptions SimpleGridOptions { get; } = new();
        private PagerOptions PagerOptions { get; } = new();
        private Sort<T> Sort { get; } = new();
        private List<Column<T>> Columns { get; set; } = new();
        private List<Search<T>> SimpleGridSearches { get; } = new();
        private GridCssOptions CssOptions { get; } = new();
        private List<RowModifier> RowModifiers { get; } = new();
        private bool IsSortable { get; set; } = false;
        private bool IsHeaderEnabled { get; set; } = true;
        private string FooterHtml { get; set; }

        private class RowModifier
        {
            public Func<T, bool> RowFilter { get; init; }
            public CssBuilder CssBuilder { get; } = new();
        }

        public SimpleGrid<T> NoHeaders()
        {
            IsHeaderEnabled = false;
            return this;
        }

        /// <summary>
        /// Add a column to the table.
        /// </summary>
        /// <param name="columnBuilder">Callback to use the ColumnBuilder to configure the column.</param>
        public SimpleGrid<T> AddColumn(Action<ColumnBuilder<T>> columnBuilder)
        {
            var column = new Column<T>();
            var builder = new ColumnBuilder<T>(column, Html);
            columnBuilder.Invoke(builder);
            Columns.Add(column);
            return this;
        }

        /// <summary>
        /// Add a column for a model expression.
        /// This automatically adds both a Header and Display and makes the column available to Sort.
        /// </summary>
        public SimpleGrid<T> AddColumnFor(Expression<Func<T, object>> expression)
        {
            var column = new Column<T>();
            var builder = new ColumnBuilder<T>(column, Html);
            builder.For(expression);
            Columns.Add(column);
            return this;
        }

        /// <summary>
        /// Add a TwoColumnGrid inside a SimpleGrid
        /// </summary>
        public SimpleGrid<T> AddColumnForTwoColumnGrid(
            Action<ColumnBuilder<T>> columnBuilder, 
            Action<TwoColumnGridBuilder, T> gridBuilder)
            => AddColumn(col =>
            {
                col
                    .DisplayAs(model =>
                    {
                        var grid = Html.TwoColumnGrid();
                        gridBuilder(grid, model);
                        return grid.RenderToString();
                    })
                    .Encoded(false);

                columnBuilder(col);
            });

        /// <summary>
        /// Add a SimpleGrid inside a SimpleGrid
        /// </summary>
        public SimpleGrid<T> AddColumnForSimpleGrid<TGrid>(
            Action<ColumnBuilder<T>> columnBuilder, 
            Func<T, IQueryable<TGrid>> modelBuilder, 
            Action<SimpleGrid<TGrid>> gridBuilder)
            where TGrid : class
            => AddColumn(col =>
            {
                col
                    .DisplayAs(model =>
                    {
                        IQueryable<TGrid> gridModel = modelBuilder(model);
                        var grid = Html.SimpleGrid(gridModel);
                        gridBuilder(grid);
                        return grid.RenderToString();
                    })
                    .Encoded(false);

                columnBuilder(col);
            });

        /// <summary>
        /// Add columns from the model via Reflection
        /// This is a quick and dirty way to quickly build the grid out with all properties of the Model
        /// </summary>
        public SimpleGrid<T> AddColumnsForModel()
        {
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                Expression<Func<T, object>> expression = ExpressionHelpers.BuildGetterLambda<T>(property);
                DisplayAttribute display = expression.GetAttribute<DisplayAttribute, T, object>();

                // Skip if DisplayAttribute.AutoGenerateField is turned off
                bool? autoGen = display?.GetAutoGenerateField(); 
                if (autoGen.HasValue && !autoGen.Value)
                    continue;
                
                Column<T> column = new();
                ColumnBuilder<T> builder = new(column, Html);
                builder.For(expression);
                Columns.Add(column);

                // Apply DisplayAttribute.Order
                int? order = display?.GetOrder();
                if (order.HasValue)
                    column.Order = order.Value;
            }
            return this;
        }

        /// <summary>
        /// Removed a column based on the header name
        /// This is the "rendered" header name
        /// </summary>
        public SimpleGrid<T> RemoveColumn(string header)
        {
            for (int i = 0; i < Columns.Count; i++)
                if (Columns[i].HeaderValue == header)
                    return RemoveColumn(i);
            return this;
        }

        /// <summary>
        /// Remove column on a zero-based index
        /// </summary>
        public SimpleGrid<T> RemoveColumn(int ndx)
        {
            Columns.RemoveAt(ndx);
            return this;
        }

        /// <summary>
        /// Rearrange the Order of the columns based on the Order property of DisplayAttribute
        /// Note that this only affects columns added from "AddColumnsForModel", and columns added
        /// after this call will be ordered as they are added
        /// </summary>
        public SimpleGrid<T> OrderColumns()
        {
            Columns = Columns
                .OrderBy(c => c.Order)
                .ToList();
            return this;
        }

        /// <summary>
        /// Add a Search function to the grid.
        /// The first AddSearch will add the Search Form
        /// </summary>
        /// <param name="name">A name to be populated in the Search Form dropdown</param>
        /// <param name="searchFilter">An function that accepts the queryable and query string
        /// the user typed in and returns a filtered query</param>
        public SimpleGrid<T> AddSearch(string name, Func<IQueryable<T>, string, IQueryable<T>> searchFilter)
        {
            SimpleGridSearches.Add(new Search<T>
            {
                SearchFilter = searchFilter,
                Name = name
            });
            return this;
        }

        /// <summary>
        /// Add search on an Enumerated field type
        /// The input value string from the user will be attempted to match the enumeration provided
        /// Then the caller provides the Search filter that handles the correponding enum value
        /// TODO: attempt to match the Display names as well!
        /// </summary>
        /// <typeparam name="T">Grid Entity Type</typeparam>
        /// <typeparam name="TEnum">Enumeration type</typeparam>
        /// <param name="name">Name of search to display in the dropdown</param>
        /// <param name="searchFilter">Enum search filter Func</param>
        public SimpleGrid<T> AddEnumSearch<TEnum>(
            string name,
            Func<IQueryable<T>, TEnum, IQueryable<T>> searchFilter)
            where TEnum : struct, IConvertible
            => AddSearch(
                name: name,
                searchFilter: (source, query) =>
                {
                    query = query.Replace(" ", "").ToLower();

                    return Enum
                        .GetNames(typeof(TEnum))
                        .TryFirst(e => e.ToLower().Contains(query), out string name)
                            ? searchFilter(source, (TEnum)Enum.Parse(typeof(TEnum), name))
                            : source.NoResults();
                });


        /// <summary>
        /// Add custom css classes and styles to various elements of the Grid.
        /// </summary>
        public SimpleGrid<T> Css(Action<GridCssOptions> cssOptionsSetter)
        {
            cssOptionsSetter.Invoke(CssOptions);
            return this;
        }

        /// <summary>
        /// Add a CSS class to the TABLE element
        /// </summary>
        public SimpleGrid<T> AddCssClass(string cssClass) => Css(options => options.Table.AddClass(cssClass));

        /// <summary>
        /// Add a CSS style to the TABLE element
        /// </summary>
        public SimpleGrid<T> AddCssStyle(string cssStyle) => Css(options => options.Table.AddStyle(cssStyle));

        /// <summary>
        /// Specify an expression to sort the Grid by default
        /// Once a column sort is selected, this will be the secondary sort
        /// </summary>
        public SimpleGrid<T> DefaultSortBy(Expression<Func<T, object>> expression, SortOrder sortOrder = SortOrder.Ascending)
        {
            Sort.Expression = expression;
            Sort.SortOrder = sortOrder;
            return this;
        }

        /// <summary>
        /// Enable/Disable column sorts
        /// When enabled, columns with "Expressions" will become sortable unless 
        /// explicitly disabled in the specific column.
        /// </summary>
        public SimpleGrid<T> Sortable(bool enable = true)
        {
            IsSortable = enable;
            return this;
        }

        /// <summary>
        /// Add CSS to to a Row (TR element) if a certain condition is met
        /// </summary>
        /// <param name="rowFilter">Function to decide if the Row should be "modified" with custom CSS</param>
        /// <param name="cssSetter">Action to specify the CSS changes to make</param>
        public SimpleGrid<T> AddRowModifier(Func<T, bool> rowFilter, Action<CssBuilder> cssSetter)
        {
            var rowAction = new RowModifier
            {
                RowFilter = rowFilter,
            };
            cssSetter(rowAction.CssBuilder);
            RowModifiers.Add(rowAction);
            return this;
        }

        /// <summary>
        /// Specify Grid Options
        /// </summary>
        public SimpleGrid<T> Options(Action<SimpleGridOptions> optionsSetter)
        {
            optionsSetter.Invoke(SimpleGridOptions);
            return this;
        }

        /// <summary>
        /// Add a Pager to the Grid
        /// </summary>
        /// <param name="pagerOptionsSetter">Action to set Pager options like top/bottom and rows per page.</param>
        public SimpleGrid<T> AddPager(Action<PagerOptions> pagerOptionsSetter = null)
        {
            PagerOptions.Enabled = true;
            pagerOptionsSetter?.Invoke(PagerOptions);
            return this;
        }

        public SimpleGrid<T> Footer(string footerHtml)
        {
            FooterHtml = footerHtml;
            return this;
        }

        /// <summary>
        /// Perform grid building operations only if a condition is met
        /// </summary>
        public SimpleGrid<T> If(bool condition, Action<SimpleGrid<T>> conditionalAction)
        {
            if (condition)
                conditionalAction(this);
            return this;
        }

        public IHtmlContent Render()
        {
            SimpleGridViewModel vm = PrepareRender();
            return Html.Partial(partialViewName: "_SHGrid", model: vm);
        }

        public async Task<IHtmlContent> RenderAsync()
        {
            SimpleGridViewModel vm = PrepareRender();
            return await Html.PartialAsync(partialViewName: "_SHGrid", model: vm);
        }

        public string RenderToString()
        {
            SimpleGridViewModel vm = PrepareRender();
            return Html.RenderPartialToString(partialViewName: "_SHGrid", model: vm);
        }
    }
}
