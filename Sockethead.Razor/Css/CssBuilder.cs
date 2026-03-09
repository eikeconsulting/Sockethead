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

        public CssBuilder RemoveClass(string cssClass)
        {
            // Handle entries that contain multiple space-separated classes
            // e.g. RemoveClass("table") on entry "table-striped table-sm table" removes only
            // the exact token "table", leaving "table-striped table-sm"
            for (int i = CssClasses.Count - 1; i >= 0; i--)
            {
                string entry = CssClasses[i];
                if (entry == cssClass)
                {
                    CssClasses.RemoveAt(i);
                    continue;
                }

                // If the entry contains spaces, split and filter out the target class
                if (entry.Contains(' '))
                {
                    string[] parts = entry.Split(' ');
                    string filtered = string.Join(" ", parts.Where(p => p != cssClass));
                    if (filtered != entry)
                    {
                        if (string.IsNullOrWhiteSpace(filtered))
                            CssClasses.RemoveAt(i);
                        else
                            CssClasses[i] = filtered;
                    }
                }
            }

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
