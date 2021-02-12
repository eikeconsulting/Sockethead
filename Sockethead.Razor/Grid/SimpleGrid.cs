using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sockethead.Razor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sockethead.Razor.Grid
{
    public class SimpleGrid<T> : SimpleGridBase where T : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="html">HTML Helper</param>
        /// <param name="source">Data Source</param>
        public SimpleGrid(IHtmlHelper html, IQueryable<T> source)
        {
            CssClasses.Add("table");
            Html = html;
            Source = source;
            State = new State(Html.ViewContext.HttpContext.Request);
        }

        private IHtmlHelper Html { get; }
        private IQueryable<T> Source { get; }
        private State State { get; }
        private Options Options { get; } = new Options();
        private SimpleGridPagerOptions PagerOptions { get; } = new SimpleGridPagerOptions();
        private Sort<T> Sort { get; } = new Sort<T>();
        private List<Column<T>> Columns { get; } = new List<Column<T>>();
        private List<SimpleGridSearch> SimpleGridSearches { get; } = new List<SimpleGridSearch>();

        public SimpleGrid<T> AddColumn(Action<Column<T>> columnBuilder)
        {
            var column = new Column<T>();
            columnBuilder.Invoke(column);
            Columns.Add(column);
            return this;
        }

        public SimpleGrid<T> AddColumnFor(Expression<Func<T, object>> expression)
        {
            var column = new Column<T>();
            column.For(expression);
            Columns.Add(column);
            return this;
        }

        public SimpleGrid<T> AddColumnsFromModel()
        {
            foreach (var property in typeof(T).GetProperties())
                AddColumnFor(ExpressionHelpers.GenerateGetterLambda<T>(property));
            return this;
        }

        public SimpleGrid<T> AddSearch(string name, Func<IQueryable<T>, string, IQueryable<T>> searchFilter)
        {
            SimpleGridSearches.Add(new SimpleGridSearch
            {
                SearchFilter = searchFilter,
                Name = name
            });
            return this;
        }

        public SimpleGrid<T> AddCssClass(string cssClass)
        {
            CssClasses.Add(cssClass);
            return this;
        }

        public SimpleGrid<T> AddCssStyle(string cssStyle)
        {
            CssStyles.Add(cssStyle);
            return this;
        }

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
                if (enable)
                {
                    if (column.Sort.IsEnabled && 
                        column.Sort.Expression == null && 
                        column.Expression != null)
                        column.Sortable(true);
                }
                else
                {
                    column.Sortable(false);
                }
            }
            return this;
        }

        public SimpleGrid<T> SetOptions(Action<Options> optionsSetter)
        {
            optionsSetter.Invoke(Options);
            return this;
        }

        public SimpleGrid<T> AddPager(Action<SimpleGridPagerOptions> pagerOptionsSetter = null)
        {
            PagerOptions.Enabled = true;
            pagerOptionsSetter?.Invoke(PagerOptions);
            return this;
        }

        public async Task<IHtmlContent> RenderAsync()
        {
            IQueryable<T> query = Source;

            var vm = new SimpleGridViewModel
            {
                Css = Css(),
                Options = Options,
                PagerOptions = PagerOptions,
            };

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
            query = Sort<T>.ApplySorts(sorts, query);

            // build pager view model
            if (vm.PagerOptions.Enabled)
                vm.PagerModel = State.BuildPagerModel(totalRecords: query.Count(), rowsPerPage: vm.PagerOptions.RowsPerPage);

            // build search view model
            if (SimpleGridSearches.Any())
            {
                vm.SimpleGridSearchViewModel = new SimpleGridSearchViewModel
                {
                    RedirectUrl = State.BuildResetUrl(),
                    SearchFilterNames = SimpleGridSearches.Select((search, i) => new SelectListItem
                    {
                        Text = search.Name,
                        Value = (i + 1).ToString(),
                        Selected = State.SearchNdx == i + 1,
                    }).ToList(),
                    SearchNdx = State.SearchNdx.ToString(),
                };
            }

            // build column labels
            int ndx = 0;
            foreach (var col in Columns)
            {
                ndx++;

                col.LabelDetails.Display = col.LabelRender();
                if (!col.Sort.IsActive)
                    continue;

                SortOrder sortOrder = col.Sort.SortOrder;

                if (ndx == State.SortColumn)
                {
                    col.LabelDetails.CurrentSortOrder = State.SortOrder;
                    sortOrder = Sort<T>.Flip(State.SortOrder);
                }

                col.LabelDetails.SortUrl = State.BuildSortUrl(ndx, sortOrder);
            }

            // resolve the data (rows)
            int rowsToTake = vm.PagerOptions.Enabled ? vm.PagerOptions.RowsPerPage : vm.Options.MaxRows;
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

        public class SimpleGridSearch
        {
            public Func<IQueryable<T>, string, IQueryable<T>> SearchFilter { get; set; }
            public string Name { get; set; }
        }
    }
}
