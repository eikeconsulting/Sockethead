using Microsoft.AspNetCore.Mvc.Rendering;
using Sockethead.Razor.Pager;
using System;
using System.Collections.Generic;

namespace Sockethead.Razor.Grid
{
    public class SimpleGridViewModel
    {
        public GridCssViewModel Css { get; set; }

        public Func<object, string> GetRowCss;

        public SimpleGridOptions Options { get; set; }

        public PagerOptions PagerOptions { get; set; }

        public PagerModel PagerModel { get; set; }

        public SimpleGridSearchViewModel SimpleGridSearchViewModel { get; set; }

        public List<object> Rows { get; set; }

        public ISimpleGridColumn[] Columns { get; set; }
    }

    public class SimpleGridAjaxViewModel
    {
        public string Endpoint { get; set; }
        public string Id { get; set; }
        public bool DisplaySearchField { get; set; }
    }

    public class GridCssViewModel
    {
        public string TableCss { get; set; }
        public string HeaderCss { get; set; }
        public string RowCss { get; set; }
    }

    public class SimpleGridSearchViewModel
    {
        public string RedirectUrl { get; set; }
        public string Query { get; set; }
        public List<SelectListItem> SearchFilterNames { get; set; }
        public string SearchNdx { get; set; }
    }
}
