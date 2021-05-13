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
        private StateFields Fields { get; }

        public State(HttpRequest request, string id = null)
        {
            Request = request;

            Fields = new StateFields(id);

            var query = Request.QueryParamDictionary();

            BaseUrl = query.ContainsKey(Fields.BaseUrlName) ? query[Fields.BaseUrlName] : null;

            PageNum = query.ContainsKey(Fields.PageNumName) && int.TryParse(query[Fields.PageNumName], out int page) ? page : 1;
            RowsPerPage = query.ContainsKey(Fields.RowsPerPageName) && int.TryParse(query[Fields.RowsPerPageName], out int rpp) ? rpp : null;

            SortColumn = query.ContainsKey(Fields.SortColumnName) && int.TryParse(query[Fields.SortColumnName], out int sort) ? sort : 0;
            SortOrder = query.ContainsKey(Fields.SortOrderName) && int.TryParse(query[Fields.SortOrderName], out int sortOrder) ? (SortOrder)sortOrder : SortOrder;

            SearchQuery = query.ContainsKey(Fields.SearchQueryName) ? query[Fields.SearchQueryName] : null;
            SearchNdx = query.ContainsKey(Fields.SearchNdxName) && int.TryParse(query[Fields.SearchNdxName], out int searchNdx) ? searchNdx : 0;
        }

        public string BuildPageUrl(int pageNum, int? rowsPerPage = null)
        {
            var q = new Dictionary<string, string>
            {
                [Fields.PageNumName] = pageNum.ToString()
            };

            if (rowsPerPage.HasValue)
                q[Fields.RowsPerPageName] = rowsPerPage.Value.ToString();

            return Request.UrlUpdateQuery(q);
        }

        public string BuildSortUrl(int columnId, SortOrder sortOrder)
        {
            var q = new Dictionary<string, string>
            {
                [Fields.PageNumName] = null,
                [Fields.SortColumnName] = columnId.ToString(),
                [Fields.SortOrderName] = ((int)sortOrder).ToString(),
            };
            return Request.UrlUpdateQuery(q);
        }

        public string BuildSearchUrl(string query, int ndx)
        {
            var q = EmptyGridParameters();
            q[Fields.SearchQueryName] = query.Substring(0, 50);
            q[Fields.SearchNdxName] = ndx.ToString();
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

        private Dictionary<string, string> EmptyGridParameters()
            => new Dictionary<string, string>
            {
                [Fields.PageNumName] = null,
                [Fields.RowsPerPageName] = null,
                [Fields.SortColumnName] = null,
                [Fields.SortOrderName] = null,
                [Fields.SearchQueryName] = null,
                [Fields.SearchNdxName] = null,
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

        public string AppendSearchParameters(string url, string query, string searchNdx)
        {
            string separator = url.Contains('?') ? "&" : "?";
            return $"{url}{separator}{Fields.SearchQueryName}={query}&{Fields.SearchNdxName}={searchNdx}";
        }
    }
}
