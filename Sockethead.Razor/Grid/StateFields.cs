namespace Sockethead.Razor.Grid
{
    /// <summary>
    /// Helper class for State
    /// </summary>
    internal class StateFields
    {
        private const string _BaseUrlName = "grid-base-url";
        private const string _PageNumName = "grid-page";
        private const string _RowsPerPageName = "grid-rows-per-page";
        private const string _SortColumnName = "grid-sort-column";
        private const string _SortOrderName = "grid-sort-order";
        private const string _SearchQueryName = "grid-search-query";
        private const string _SearchNdxName = "grid-search-ndx";

        public string BaseUrlName { get; set; } = _BaseUrlName;
        public string PageNumName { get; set; } = _PageNumName;
        public string RowsPerPageName { get; set; } = _RowsPerPageName;
        public string SortColumnName { get; set; } = _SortColumnName;
        public string SortOrderName { get; set; } = _SortOrderName;
        public string SearchQueryName { get; set; } = _SearchQueryName;
        public string SearchNdxName { get; set; } = _SearchNdxName;

        public StateFields(string id = null)
        {
            if (string.IsNullOrEmpty(id))
                return;

            const string from = "grid-";
            string to = $"grid-{id}-";

            BaseUrlName = BaseUrlName.Replace(from, to);
            PageNumName = PageNumName.Replace(from, to);
            RowsPerPageName = RowsPerPageName.Replace(from, to);
            SortColumnName = SortColumnName.Replace(from, to);
            SortOrderName = SortOrderName.Replace(from, to);
            SearchQueryName = SearchQueryName.Replace(from, to);
            SearchNdxName = SearchNdxName.Replace(from, to);
        }
    }
}
