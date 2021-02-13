using Microsoft.AspNetCore.Mvc.Rendering;
using Sockethead.Razor.Pager;
using System.Collections.Generic;

namespace Sockethead.Razor.Grid
{
    public class SimpleGridViewModel
    {
        public string Css { get; set; }

        public GridOptions Options { get; set; }

        public PagerOptions PagerOptions { get; set; }

        public PagerModel PagerModel { get; set; }

        public SimpleGridSearchViewModel SimpleGridSearchViewModel { get; set; }

        public List<object> Rows { get; set; }

        public ISimpleGridColumn[] Columns { get; set; }
    }

    public class SimpleGridSearchViewModel
    {
        public string RedirectUrl { get; set; }
        public string Query { get; set; }
        public List<SelectListItem> SearchFilterNames { get; set; }
        public string SearchNdx { get; set; }
    }
}
