using Sockethead.Razor.Css;

namespace Sockethead.Razor.Grid
{
    public class SimpleGridOptions
    {
        public string GridViewName { get; set; } = "_SHGrid";
        public string SearchViewName { get; set; } = "_SHGridSearch";
        public string TableViewName { get; set; } = "_SHGridTable";
        public int MaxRows { get; set; } = 5000;
    }

    public class GridCssOptions
    {
        public CssBuilder Table { get; } = new CssBuilder().AddClass("table");
        public CssBuilder Header { get; } = new CssBuilder();
        public CssBuilder Row { get; } = new CssBuilder();

        public void ClearAll()
        {
            Table.Clear();
            Header.Clear();
            Row.Clear();
        }
    }
}
