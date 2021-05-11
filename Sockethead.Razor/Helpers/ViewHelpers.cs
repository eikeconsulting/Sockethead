using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sockethead.Razor.Helpers
{
    public static class ViewHelpers
    {
        public static ViewResult SetTitle(this ViewResult viewResult, string title)
        {
            viewResult.ViewData["Title"] = title;
            return viewResult;
        }
    }
}
