using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sockethead.Razor.Css;
using Sockethead.Razor.Helpers;
using Sockethead.Razor.Pager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sockethead.Razor.Grid
{
    public class SimpleGrid<T> where T : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="html">HTML Helper</param>
        /// <param name="source">Data Source</param>
        public SimpleGrid(IHtmlHelper html, IQueryable<T> source)
        {
            Html = html;
            Source = source;
            State = new State(Html.ViewContext.HttpContext.Request);
        }

        private IHtmlHelper Html { get; }
        private IQueryable<T> Source { get; }
        private State State { get; }
        private SimpleGridOptions Options { get; } = new SimpleGridOptions();
        private PagerOptions PagerOptions { get; } = new PagerOptions();
        private Sort<T> Sort { get; } = new Sort<T>();
        private List<Column<T>> Columns { get; } = new List<Column<T>>();
        private List<Search<T>> SimpleGridSearches { get; } = new List<Search<T>>();
        private GridCssOptions CssOptions { get; } = new GridCssOptions();

        public SimpleGrid<T> AddColumn(Action<ColumnBuilder<T>> action)
        {
            var column = new Column<T>();
            var builder = new ColumnBuilder<T>(column);
            action.Invoke(builder);
            Columns.Add(column);
            return this;
        }

        public SimpleGrid<T> AddColumnFor(Expression<Func<T, object>> expression)
        {
            var column = new Column<T>();
            var builder = new ColumnBuilder<T>(column);
            builder.For(expression);
            Columns.Add(column);
            return this;
        }

        public SimpleGrid<T> AddColumnsFromModel()
        {
            foreach (var property in typeof(T).GetProperties())
                AddColumnFor(ExpressionHelpers.GenerateGetterLambda<T>(property));
            return this;
        }

        public SimpleGrid<T> AddSearch(string name, Expression<Func<T, string, bool>> modelFilter)
            => AddSearch(name,
                searchFilter: (source, query) => source.Where(model => modelFilter.Compile().Invoke(model, query)));

        public SimpleGrid<T> AddSearch(string name, Func<IQueryable<T>, string, IQueryable<T>> searchFilter)
        {
            SimpleGridSearches.Add(new Search<T>
            {
                SearchFilter = searchFilter,
                Name = name
            });
            return this;
        }

        public SimpleGrid<T> AddCss(Action<GridCssOptions> cssOptionsSetter)
        {
            cssOptionsSetter.Invoke(CssOptions);
            return this;
        }
        public SimpleGrid<T> AddCssClass(string cssClass) => AddCss(options => options.Table.AddClass(cssClass));
        public SimpleGrid<T> AddCssStyle(string cssStyle) => AddCss(options => options.Table.AddStyle(cssStyle));

        public SimpleGrid<T> DefaultSortBy(Expression<Func<T, object>> expression, SortOrder sortOrder = SortOrder.Ascending)
        {
            Sort.Expression = expression;
            Sort.SortOrder = sortOrder;
            return this;
        }

        public SimpleGrid<T> Sortable(bool enable = true)
        {
            foreach (var column in Columns)
            {
                var builder = new ColumnBuilder<T>(column);

                if (enable)
                {
                    if (column.Sort.IsEnabled &&
                        column.Sort.Expression == null &&
                        column.Expression != null)
                        builder.Sortable(true);
                }
                else
                {
                    builder.Sortable(false);
                }
            }
            return this;
        }

        public class RowAction
        {
            public Func<T, bool> RowFilter { get; set; }
            public CssBuilder CssBuilder { get; set; } = new CssBuilder();
        }

        private List<RowAction> RowActionList { get; } = new List<RowAction>();

        //public SimpleGrid<T> AddRowAction(Action<RowAction> rowActionSetter)
        public SimpleGrid<T> AddRowAction(Func<T, bool> rowFilter, Action<CssBuilder> cssSetter)
        {
            var rowAction = new RowAction
            {
                RowFilter = rowFilter,
            };
            cssSetter(rowAction.CssBuilder);
            RowActionList.Add(rowAction);

            /*
            var rowAction = new RowAction();
            rowActionSetter(rowAction);
            RowActionList.Add(rowAction);
            */
            return this;
        }

        public SimpleGrid<T> SetOptions(Action<SimpleGridOptions> optionsSetter)
        {
            optionsSetter.Invoke(Options);
            return this;
        }

        public SimpleGrid<T> AddPager(Action<PagerOptions> pagerOptionsSetter = null)
        {
            PagerOptions.Enabled = true;
            pagerOptionsSetter?.Invoke(PagerOptions);
            return this;
        }

        private IQueryable<T> BuildQuery()
        {
            IQueryable<T> query = Source;

            // apply search to query
            if (!string.IsNullOrEmpty(State.SearchQuery) &&
                State.SearchNdx > 0 &&
                State.SearchNdx <= SimpleGridSearches.Count)
                query = SimpleGridSearches[State.SearchNdx - 1].SearchFilter(query, State.SearchQuery);

            // apply sort(s) to query
            var sortColumn = State.SortColumn > 0 && State.SortColumn <= Columns.Count ? Columns[State.SortColumn - 1] : null;
            var sorts = new List<Sort<T>>();
            if (sortColumn != null && sortColumn.Sort.IsActive)
            {
                sortColumn.Sort.SortOrder = State.SortOrder; // kludge
                sorts.Add(sortColumn.Sort);
            }
            sorts.Add(Sort);
            return Sort<T>.ApplySorts(sorts, query);
        }

        public async Task<IHtmlContent> RenderAsync()
        {
            IQueryable<T> query = BuildQuery();

            int totalRecords = query.Count();
            PagerOptions.Enabled = PagerOptions.Enabled && 
                (PagerOptions.RowsPerPage < totalRecords || !PagerOptions.HideIfTooFewRows);

            var vm = new SimpleGridViewModel
            {
                Css = new GridCssViewModel
                {
                    TableCss = CssOptions.Table.ToString(),
                    HeaderCss = CssOptions.Header.ToString(),
                    RowCss = CssOptions.Row.ToString(),
                },

                GetRowCss = row => RowActionList.FirstOrDefault(rc => rc.RowFilter(row as T))?.CssBuilder.ToString(),

                Options = Options,

                // build pager view model
                PagerOptions = PagerOptions,
                PagerModel = PagerOptions.Enabled
                    ? State.BuildPagerModel(
                        totalRecords: totalRecords, 
                        rowsPerPage: PagerOptions.RowsPerPage)
                    : null,

                // build search view model
                SimpleGridSearchViewModel = SimpleGridSearches.Any()
                    ? new SimpleGridSearchViewModel
                    {
                        RedirectUrl = State.BuildResetUrl(),
                        SearchFilterNames = SimpleGridSearches.Select((search, i) => new SelectListItem
                        {
                            Text = search.Name,
                            Value = (i + 1).ToString(),
                            Selected = State.SearchNdx == i + 1,
                        }).ToList(),
                        SearchNdx = State.SearchNdx.ToString(),
                    }
                    : null,
            };

            // build column headers
            int ndx = 0;
            foreach (var col in Columns)
            {
                ndx++;

                col.HeaderDetails.Display = col.HeaderRender();
                if (!col.Sort.IsActive)
                    continue;

                SortOrder sortOrder = col.Sort.SortOrder;

                if (ndx == State.SortColumn)
                {
                    col.HeaderDetails.CurrentSortOrder = State.SortOrder;
                    sortOrder = Sort<T>.Flip(State.SortOrder);
                }

                col.HeaderDetails.SortUrl = State.BuildSortUrl(ndx, sortOrder);
            }

            // resolve the data (rows)
            int rowsToTake = PagerOptions.Enabled ? PagerOptions.RowsPerPage : Options.MaxRows;
            vm.Rows = query
                .Skip((State.PageNum - 1) * rowsToTake)
                .Take(rowsToTake)
                .Cast<object>()
                .ToList();

            // build array of generic columns (not tied to the type of the model)
            vm.Columns = Columns.Cast<ISimpleGridColumn>().ToArray();

            // render it!
            return await Html.PartialAsync("_SHGrid", vm);
        }
    }
}
