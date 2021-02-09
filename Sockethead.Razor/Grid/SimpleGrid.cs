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
            State = new SimpleGridState(Html.ViewContext.HttpContext.Request);
        }

        private SimpleGridState State { get; }
        private SimpleGridOptions Options { get; } = new SimpleGridOptions();
        private SimpleGridPagerOptions PagerOptions { get; set; } = null;
        private SimpleGridSort<T> Sort { get; } = new SimpleGridSort<T>();
        private IHtmlHelper Html { get; set; }
        private IQueryable<T> Source { get; set; }
        private List<SimpleGridColumn<T>> Columns { get; } = new List<SimpleGridColumn<T>>();
        private List<SimpleGridSearch> SimpleGridSearches { get; } = new List<SimpleGridSearch>();

        public SimpleGrid<T> AddColumn(Action<SimpleGridColumn<T>> columnBuilder)
        {
            var column = new SimpleGridColumn<T>();
            columnBuilder.Invoke(column);
            Columns.Add(column);
            return this;
        }

        public SimpleGrid<T> AddColumnFor(Expression<Func<T, object>> expression)
        {
            var column = new SimpleGridColumn<T>();
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
                    if (!column.Sort.IsSortable && column.Expression != null)
                        column.Sortable(true);
                }
                else
                {
                    column.Sortable(false);
                }
            }
            return this;
        }

        public SimpleGrid<T> SetOptions(Action<SimpleGridOptions> optionsSetter)
        {
            optionsSetter.Invoke(Options);
            return this;
        }

        public SimpleGrid<T> AddPager(Action<SimpleGridPagerOptions> pagerOptionsSetter = null)
        {
            PagerOptions = new SimpleGridPagerOptions();
            pagerOptionsSetter?.Invoke(PagerOptions);
            return this;
        }

        public async Task<IHtmlContent> RenderAsync()
        {
            IQueryable<T> src = Source;

            var vm = new SimpleGridViewModel
            {
                Css = Css(),
                Options = Options,
                PagerOptions = PagerOptions,
            };

            // search
            if (!string.IsNullOrEmpty(State.SearchQuery) &&
                State.SearchNdx > 0 && 
                State.SearchNdx <= SimpleGridSearches.Count)
                src = SimpleGridSearches[State.SearchNdx - 1].SearchFilter(src, State.SearchQuery);

            // sort
            var sortColumn = State.SortColumn > 0 && State.SortColumn <= Columns.Count ? Columns[State.SortColumn - 1] : null;
            if (sortColumn != null && sortColumn.Sort.IsSortable)
            {
                sortColumn.Sort.SortOrder = State.SortOrder; // kludge
                src = sortColumn.Sort.ApplyTo(src, isThenBy: false);
                src = Sort.ApplyTo(src, isThenBy: true);
            }
            else
            {
                src = Sort.ApplyTo(src, isThenBy: false);
            }

            // pager
            if (vm.PagerOptions != null)
                vm.PagerModel = State.BuildPagerModel(totalRecords: src.Count(), rowsPerPage: vm.PagerOptions.RowsPerPage);

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

            vm.ColumnLabels = Columns.Select((col, ndx) =>
            {
                string label = col.LabelRender();
                if (!col.Sort.IsSortable)
                    return label;

                SortOrder sortOrder = ndx + 1 == State.SortColumn
                    ? SimpleGridSort<T>.Flip(State.SortOrder)
                    : col.Sort.SortOrder;

                return $"<a href='{State.BuildSortUrl(ndx + 1, sortOrder)}'>{label}</a>";
            }).ToArray();

            int rowsToTake = vm.PagerOptions == null ? vm.Options.MaxRows : vm.PagerOptions.RowsPerPage;
            vm.Rows = src
                .Skip((State.PageNum - 1) * rowsToTake)
                .Take(rowsToTake)
                .Cast<object>()
                .ToList();

            vm.Columns = Columns.Cast<ISimpleGridColumn>().ToArray();

            return await Html.PartialAsync("_Grid", vm);
        }

        public class SimpleGridSearch
        {
            public Func<IQueryable<T>, string, IQueryable<T>> SearchFilter { get; set; }
            public string Name { get; set; }
        }
    }

    public class SimpleGridOptions
    {
        public string PagerViewName { get; set; } = "_Pager";
        public int MaxRows { get; set; } = 5000;
    }

    public class SimpleGridPagerOptions
    {
        public int RowsPerPage { get; set; } = 20;
        public bool DisplayPagerTop { get; set; } = true;
        public bool DisplayPagerBottom { get; set; } = false;
    }
}
