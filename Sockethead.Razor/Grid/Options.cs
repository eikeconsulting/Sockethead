namespace Sockethead.Razor.Grid
{
    public class Options
    {
        public string GridViewName { get; set; } = "_SHGrid";
        public string PagerViewName { get; set; } = "_SHPager";
        public string SearchViewName { get; set; } = "_SHGridSearch";
        public string TableViewName { get; set; } = "_SHGridTable";
        public int MaxRows { get; set; } = 5000;
    }

    public class SimpleGridPagerOptions
    {
        public bool Enabled { get; set; } = false;
        public int RowsPerPage { get; set; } = 20;
        public bool DisplayPagerTop { get; set; } = true;
        public bool DisplayPagerBottom { get; set; } = false;
    }
}
