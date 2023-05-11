using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sockethead.Razor.Forms
{
    public class FormRowViewModel
    {
        public FormOptions FormOptions { get; set; }
        public FormRowOptions FormRowOptions { get; set; }

        public IEnumerable<SelectListItem> SelectListItems { get; set; }
        
        public Func<SelectListItem, IHtmlContent> RenderSelectListItem { get; set; }

        public Func<string, IHtmlContent> Label { get; set; }
        public IHtmlContent Input { get; set; }
        public IHtmlContent ValidationMessage { get; set; }
    }
}