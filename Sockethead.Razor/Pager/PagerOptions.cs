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

        public PagerOptions Top()
        {
            DisplayPagerTop = true;
            DisplayPagerBottom = false;
            return this;
        }

        public PagerOptions Botttom()
        {
            DisplayPagerTop = false;
            DisplayPagerBottom = true;
            return this;
        }

        public PagerOptions TopAndBottom()
        {
            DisplayPagerTop = true;
            DisplayPagerBottom = true;
            return this;
        }

    }
}
