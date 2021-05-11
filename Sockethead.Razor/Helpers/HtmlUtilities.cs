using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sockethead.Razor.Helpers
{
    public static class HtmlUtilities
    {
        public static string BuildList(IEnumerable<object> items) 
            => (items == null || !items.Any()) 
                ? "" 
                : $"<ul>\n{string.Join("\n", items.Select(line => $"<li>{line}</li>"))}</ul>\n";


        public static string BuildImage(string url)
            => $"<img src='{url}' />";

    }


}
