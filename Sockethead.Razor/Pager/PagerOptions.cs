namespace Sockethead.Razor.Pager
{
    public class PagerOptions
    {
        public string PagerViewName { get; set; } = "_SHPager";

        public bool Enabled { get; set; } = false;
        public int RowsPerPage { get; set; } = 20;
        public bool DisplayPagerTop { get; set; } = true;
        public bool DisplayPagerBottom { get; set; } = false;

        public bool HideIfTooFewRows { get; set; } = true;

        public PagerOptions Top(bool enable)
        {
            DisplayPagerTop = enable;
            return this;
        }

        public PagerOptions Botttom(bool enable)
        {
            DisplayPagerBottom = enable;
            return this;
        }
    }
}
