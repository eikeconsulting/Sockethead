using System;
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
            for (int i = CssClasses.Count - 1; i >= 0; i--)
            {
                string entry = CssClasses[i];

                // Exact match — remove the whole entry
                if (entry == cssClass)
                {
                    CssClasses.RemoveAt(i);
                    continue;
                }

                // Multi-class entry (e.g. "table-striped table-sm table"):
                // split on any whitespace, remove matching tokens, rejoin
                string[] parts = entry.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    string filtered = string.Join(" ", parts.Where(p => p != cssClass));
                    if (filtered.Length == 0)
                        CssClasses.RemoveAt(i);
                    else if (filtered != entry)
                        CssClasses[i] = filtered;
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
