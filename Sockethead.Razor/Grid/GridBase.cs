using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sockethead.Razor.Grid
{
    public class GridBase
    {
        public List<string> CssClasses { get; } = new List<string>();
        public List<string> CssStyles { get; } = new List<string>();

        public string Css() 
        {
            var sb = new StringBuilder();

            if (CssClasses.Any())
                sb.Append($" class='{string.Join(" ", CssClasses)}'");

            if (CssStyles.Any())
                sb.Append($" style='{string.Join(";", CssStyles)}'");

            return sb.ToString();
        }
    }
}
