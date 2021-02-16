namespace Sockethead.Razor.Grid
{
    public class SimpleGridOptions
    {
        public string GridViewName { get; set; } = "_SHGrid";
        public string SearchViewName { get; set; } = "_SHGridSearch";
        public string TableViewName { get; set; } = "_SHGridTable";
        public int MaxRows { get; set; } = 5000;
    }
}
