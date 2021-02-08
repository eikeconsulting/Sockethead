using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sockethead.Razor.Helpers;
using Sockethead.Razor.Pager;
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
        public SimpleGrid(IHtmlHelper html, IQueryable<T> source, SimpleGridState state = null)
        {
            CssClasses.Add("table");
            Html = html;
            Source = source;
            State = state ?? new SimpleGridState();
        }

        public SimpleGridState State { get; }
        private SimpleGridOptions Options { get; set; } = new SimpleGridOptions();
        private SimpleGridPagerOptions PagerOptions { get; set; } = null;
        private SimpleGridSort<T> Sort { get; set; } = new SimpleGridSort<T>();
        public int PageNum => State.PageNum;
        public int SortColumnId => State.SortColumnId;
        private IHtmlHelper Html { get; set; }
        private IQueryable<T> Source { get; set; }
        private List<SimpleGridColumn<T>> Columns { get; } = new List<SimpleGridColumn<T>>();

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

            var sortColumn = SortColumnId > 0 && SortColumnId <= Columns.Count ? Columns[SortColumnId - 1] : null;
            if (sortColumn != null && sortColumn.IsSortable)
            {
                Source = sortColumn.Sort.ApplyTo(Source, isThenBy: false);
                Source = Sort.ApplyTo(Source, isThenBy: true);
            }
            else
            {
                Source = Sort.ApplyTo(Source, isThenBy: false);
            }

            if (PagerOptions != null)
            {
                var pagerModel = BuildPagerModel(
                    totalPages: (int)Math.Ceiling((float)Source.Count() / (float)PagerOptions.RowsPerPage));

                Html.RenderPartial(Options.PagerViewName, pagerModel);
            }

            sb.Append($"<table{Css()}>\n");

            sb.Append("<tr>\n");
            int columnId = 1;
            foreach (var col in Columns)
            {
                string label = col.LabelRender();
                if (col.IsSortable)
                    label = $"<a href='{BuildSortUrl(columnId)}'>{label}</a>";
                sb.Append($"<th>{label}</th>\n");
                columnId++;
            }
            sb.Append("</tr>\n");

            int rowsToTake = PagerOptions == null ? Options.MaxRows : PagerOptions.RowsPerPage;

            foreach (T item in Source.Skip((PageNum - 1) * rowsToTake).Take(rowsToTake).ToList())
            {
                sb.Append("<tr>\n");
                foreach (var col in Columns)
                    sb.Append($"<td{col.Css()}>{col.DisplayRender(item)}</td>\n");
                sb.Append("</tr>\n");
            }

            sb.Append("</table>\n");

            return new HtmlString(sb.ToString());
        }

        private string BuildPageUrl(int pageNum)
            => Html.ViewContext.HttpContext.Request.UrlUpdateQuery(
                new Dictionary<string, string>
                {
                    ["page"] = pageNum.ToString()
                });

        private string BuildSortUrl(int columnId)
            => Html.ViewContext.HttpContext.Request.UrlUpdateQuery(
                new Dictionary<string, string>
                {
                    ["page"] = "1",
                    ["sort"] = columnId.ToString(),
                });

        private PagerModel BuildPagerModel(int totalPages)
            => new PagerModel
            {
                FirstUrl = PageNum > 1 ? BuildPageUrl(1) : null,
                PrevUrl = PageNum > 1 ? BuildPageUrl(PageNum - 1) : null,
                NextUrl = PageNum < totalPages ? BuildPageUrl(PageNum + 1) : null,
                LastUrl = PageNum < totalPages ? BuildPageUrl(totalPages) : null,
                CurrentPage = PageNum,
                TotalPages = totalPages,
            };

        public class SimpleGridOptions
        {
            public string PagerViewName { get; set; } = "_Pager";
            public int MaxRows { get; set; } = 5000;
        }

        public class SimpleGridPagerOptions
        {
            public int RowsPerPage { get; set; } = 20;
        }
    }
}
