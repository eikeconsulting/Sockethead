using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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

        public SimpleGrid<T> AddColumnFor(Expression<Func<T, string>> expression)
        {
            var column = new SimpleGridColumn<T>();
            column.For(expression);
            Columns.Add(column);
            return this;
        }

        public SimpleGrid<T> AddSearch(Func<IQueryable<T>, string, IQueryable<T>> searchFilter, string name = null)
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

        public SimpleGrid<T> DefaultSortBy(Expression<Func<T, string>> expression, SortOrder sortOrder = SortOrder.Ascending)
        {
            Sort.Expression = expression;
            Sort.SortOrder = sortOrder;
            return this;
        }

        public SimpleGrid<T> SetOptions(Action<SimpleGridOptions> optionsSetter)
        {
            optionsSetter?.Invoke(Options);
            return this;
        }

        public SimpleGrid<T> AddPager(Action<SimpleGridPagerOptions> pagerOptionsSetter = null)
        {
            PagerOptions = new SimpleGridPagerOptions();
            pagerOptionsSetter?.Invoke(PagerOptions);
            return this;
        }

        public IHtmlContent Render()
        {
            var sb = new StringBuilder();

            // searching
            if (!string.IsNullOrEmpty(State.SearchQuery))
            {
                if (State.SearchNdx > 0 && State.SearchNdx <= SimpleGridSearches.Count)
                {
                    var search = SimpleGridSearches[State.SearchNdx - 1];
                    Source = search.SearchFilter.Invoke(Source, State.SearchQuery);
                }
            }

            // sorting
            var sortColumn = State.SortColumn > 0 && State.SortColumn <= Columns.Count ? Columns[State.SortColumn - 1] : null;
            if (sortColumn != null && sortColumn.Sort.IsSortable)
            {
                sortColumn.Sort.SortOrder = State.SortOrder; // kludge
                Source = sortColumn.Sort.ApplyTo(Source, isThenBy: false);
                Source = Sort.ApplyTo(Source, isThenBy: true);
            }
            else
            {
                if (Sort.IsSortable)
                    Source = Sort.ApplyTo(Source, isThenBy: false);
            }

            // pager
            var pagerModel = PagerOptions == null 
                ?  null 
                : State.BuildPagerModel(totalPages: (int)Math.Ceiling((float)Source.Count() / (float)PagerOptions.RowsPerPage));

            if (PagerOptions != null && PagerOptions.DisplayPagerTop)
                Html.RenderPartial(Options.PagerViewName, pagerModel);


            if (SimpleGridSearches.Any())
            {
                Html.RenderPartial("_Search");
            }

            // render table
            sb.Append($"<table{Css()}>\n");

            sb.Append("<tr>\n");
            int columnId = 1;
            foreach (var col in Columns)
            {
                string label = col.LabelRender();
                if (col.Sort.IsSortable)
                {
                    SortOrder sortOrder = columnId == State.SortColumn
                        ? SimpleGridSort<T>.Flip(State.SortOrder)
                        : col.Sort.SortOrder;
                    label = $"<a href='{State.BuildSortUrl(columnId, sortOrder)}'>{label}</a>";
                }
                sb.Append($"<th>{label}</th>\n");
                columnId++;
            }
            sb.Append("</tr>\n");

            int rowsToTake = PagerOptions == null ? Options.MaxRows : PagerOptions.RowsPerPage;

            foreach (T item in Source.Skip((State.PageNum - 1) * rowsToTake).Take(rowsToTake).ToList())
            {
                sb.Append("<tr>\n");
                foreach (var col in Columns)
                    sb.Append($"<td{col.Css()}>{col.DisplayRender(item)}</td>\n");
                sb.Append("</tr>\n");
            }

            sb.Append("</table>\n");

            if (PagerOptions != null && PagerOptions.DisplayPagerBottom)
                Html.RenderPartial(Options.PagerViewName, pagerModel);

            return new HtmlString(sb.ToString());
        }

        public class SimpleGridSearch
        {
            public Func<IQueryable<T>, string, IQueryable<T>> SearchFilter { get; set; }
            public string Name { get; set; }
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
}
