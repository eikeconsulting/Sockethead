using System.Collections.Generic;

namespace Sockethead.Razor.Pager
{
    public class PagerModel
    {
        public string FirstUrl { get; set; }
        public string NextUrl { get; set; }
        public string PrevUrl { get; set; }
        public string LastUrl { get; set; }

        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public int? TotalItems { get; set; }

        public Dictionary<int, string> RowsPerPageLinks { get; set; }

    }
}
