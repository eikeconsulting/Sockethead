using Sockethead.Razor.Css;

namespace Sockethead.Razor.Grid
{
    public class SimpleGridOptions
    {
        public string GridViewName { get; set; } = "_SHGrid";
        public string SearchViewName { get; set; } = "_SHGridSearch";
        public string TableViewName { get; set; } = "_SHGridTable";

        /// <summary>
        /// Maximum number of rows to render at one time
        /// This is a saftey feature if you are querying a huge table
        /// </summary>
        public int MaxRows { get; set; } = 5000;

        /// <summary>
        /// Message to display when there are no records to display
        /// May optionally include HTML (i.e. this won't get encoded)
        /// </summary>
        public string NoMatchingRecordsHtml { get; set; } = "No matching records.";
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
