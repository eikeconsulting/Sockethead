using Microsoft.AspNetCore.Http;
using Sockethead.Razor.Helpers;
using Sockethead.Razor.Pager;
using System.Collections.Generic;
using System.Linq;

namespace Sockethead.Razor.Grid
{
    public class State
    {
        public string BaseUrl { get; set; } = null;
        public int PageNum { get; set; } = 1;
        public int? RowsPerPage { get; set; } = null;

        public int SortColumn { get; set; } = 0;
        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
        public string SearchQuery { get; set; } = null;
        public int SearchNdx { get; set; } = 0;
        private HttpRequest Request { get; }

        public State(HttpRequest request)
        {
            Request = request;

            var query = Request.QueryParamDictionary();

            BaseUrl = query.ContainsKey(BaseUrlName) ? query[BaseUrlName] : null;

            PageNum = query.ContainsKey(PageNumName) && int.TryParse(query[PageNumName], out int page) ? page : 1;
            RowsPerPage = query.ContainsKey(RowsPerPageName) && int.TryParse(query[RowsPerPageName], out int rpp) ? rpp : null;

            SortColumn = query.ContainsKey(SortColumnName) && int.TryParse(query[SortColumnName], out int sort) ? sort : 0;
            SortOrder = query.ContainsKey(SortOrderName) && int.TryParse(query[SortOrderName], out int sortOrder) ? (SortOrder)sortOrder : SortOrder;

            SearchQuery = query.ContainsKey(SearchQueryName) ? query[SearchQueryName] : null;
            SearchNdx = query.ContainsKey(SearchNdxName) && int.TryParse(query[SearchNdxName], out int searchNdx) ? searchNdx : 0;
        }

        private const string BaseUrlName = "grid-base-url";

        private const string PageNumName = "grid-page";
        private const string RowsPerPageName = "grid-rows-per-page";

        private const string SortColumnName = "grid-sort-column";
        private const string SortOrderName = "grid-sort-order";

        private const string SearchQueryName = "grid-search-query";
        private const string SearchNdxName = "grid-search-ndx";

        public string BuildPageUrl(int pageNum, int? rowsPerPage = null)
        {
            var q = new Dictionary<string, string>
            {
                [PageNumName] = pageNum.ToString()
            };

            if (rowsPerPage.HasValue)
                q[RowsPerPageName] = rowsPerPage.Value.ToString();

            return Request.UrlUpdateQuery(q);
        }

        public string BuildSortUrl(int columnId, SortOrder sortOrder)
        {
            var q = new Dictionary<string, string>
            {
                [PageNumName] = null,
                [SortColumnName] = columnId.ToString(),
                [SortOrderName] = ((int)sortOrder).ToString(),
            };
            return Request.UrlUpdateQuery(q);
        }

        public string BuildSearchUrl(string query, int ndx)
        {
            var q = EmptyGridParameters();
            q[SearchQueryName] = query.Substring(0, 50);
            q[SearchNdxName] = ndx.ToString();
            return Request.UrlUpdateQuery(q);
        }

        public string BuildResetUrl()
        {
            var q = EmptyGridParameters();

            if (!string.IsNullOrEmpty(BaseUrl))
            {
                string separator = BaseUrl.Contains("?") ? "&" : "?";
                return $"{BaseUrl}{separator}{string.Join("&", q.Select(kvp => $"{kvp.Key}={System.Web.HttpUtility.UrlEncode(kvp.Value)}"))}";
            }

            return Request.UrlUpdateQuery(q);
        }

        private static Dictionary<string, string> EmptyGridParameters()
            => new Dictionary<string, string>
            {
                [PageNumName] = null,
                [RowsPerPageName] = null,
                [SortColumnName] = null,
                [SortOrderName] = null,
                [SearchQueryName] = null,
                [SearchNdxName] = null,
            };

        public PagerModel BuildPagerModel(int totalRecords, bool displayTotal, int rowsPerPage, int[] rowsPerPageOptions)
        {
            int totalPages = (int)System.Math.Ceiling(totalRecords / (float)rowsPerPage);
            if (totalPages < 1)
                totalPages = 1;

            var pager = new PagerModel
            {
                FirstUrl = PageNum > 1 ? BuildPageUrl(1) : null,
                PrevUrl = PageNum > 1 ? BuildPageUrl(PageNum - 1) : null,
                NextUrl = PageNum < totalPages ? BuildPageUrl(PageNum + 1) : null,
                LastUrl = PageNum < totalPages ? BuildPageUrl(totalPages) : null,
                CurrentPage = PageNum,
                TotalPages = totalPages,
            };

            if (displayTotal)
                pager.TotalItems = totalRecords;

            if (rowsPerPageOptions != null)
                pager.RowsPerPageLinks = rowsPerPageOptions.ToDictionary(rpp => rpp, rpp => BuildPageUrl(1, rpp));

            return pager;
        }

        public static string AppendSearchParameters(string url, string query, string searchNdx)
        {
            string separator = url.Contains('?') ? "&" : "?";
            return $"{url}{separator}{SearchQueryName}={query}&{SearchNdxName}={searchNdx}";
        }
    }
}
