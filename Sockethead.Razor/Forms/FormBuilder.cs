using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sockethead.Razor.Forms
{
    public class FormBuilder
    {
        private IHtmlHelper Html { get; }

        public FormBuilder(IHtmlHelper html)
        {
            Html = html;
        }

        public async Task<IHtmlContent> RenderAsync()
        {
            return await Html.PartialAsync("_SHForm", null);
        }
    }
}
