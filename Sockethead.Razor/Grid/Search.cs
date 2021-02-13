using System;
using System.Linq;

namespace Sockethead.Razor.Grid
{
    internal class Search<T>
    {
        public Func<IQueryable<T>, string, IQueryable<T>> SearchFilter { get; set; }
        public string Name { get; set; }
    }
}
