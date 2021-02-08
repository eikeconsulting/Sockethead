using System.Linq;

namespace Sockethead.Razor.Grid
{
    public class SimpleGridViewModel<T> where T : class
    {
        public IQueryable<T> Source { get; set; }
        public SimpleGridState State { get; set; }
    }
}
