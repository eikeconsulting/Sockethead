namespace Sockethead.Razor.Grid
{
    public class SimpleGridState
    {
        public int PageNum { get; set; } = 1;
        public int SortColumnId { get; set; } = 0;
        public string Search { get; set; } = null;
        public int SearchId { get; set; } = 0;
    }
}
