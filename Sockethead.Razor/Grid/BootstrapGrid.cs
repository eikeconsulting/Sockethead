using Microsoft.AspNetCore.Html;
using Sockethead.Razor.Css;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sockethead.Razor.Grid
{
    public class BootstrapGrid
    {
        internal bool UseContainer { get; set; } = false;
        public class BSColumn
        {
            public string Html { get; set; }

            public void Render(StringBuilder sb)
            {
                sb.Append($"<div class='col'>");

                sb.Append(Html);

                sb.Append("</div>");
            }
        }

        public class BSRow
        {
            readonly List<BSColumn> Columns = new List<BSColumn>();

            public BSRow AddColumn(Action<BSColumn> columnBuilder)
            {
                var column = new BSColumn();
                columnBuilder(column);
                Columns.Add(column);
                return this;
            }

            public void Render(StringBuilder sb)
            {
                sb.Append("<div class='row'>");

                foreach (var column in Columns)
                    column.Render(sb);

                sb.Append("</div>");
            }
        }

        public BootstrapGrid AddRow()
        {
            return this;
        }

        public BootstrapGrid AddContainer()
        {
            UseContainer = true;
            return this;
        }

        public string Render()
        {
            var sb = new StringBuilder();

            if (UseContainer)
                sb.Append("<div class='container'>");

            //foreach (var row in Rows)

            if (UseContainer)
                sb.Append("</div>");

            return sb.ToString();
        }

    }
}
