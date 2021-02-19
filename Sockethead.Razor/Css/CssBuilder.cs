using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sockethead.Razor.Css
{
    public class CssBuilder
    {
        public CssBuilder AddClass(string cssClass)
        {
            CssClasses.Add(cssClass);
            return this;
        }

        public CssBuilder AddStyle(string cssStyle)
        {
            CssStyles.Add(cssStyle);
            return this;
        }

        public CssBuilder Clear()
        {
            CssClasses.Clear();
            CssStyles.Clear();
            return this;
        }

        private List<string> CssClasses { get; } = new List<string>();
        private List<string> CssStyles { get; } = new List<string>();

        public override string ToString()
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
