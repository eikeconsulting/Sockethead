using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    [Area("Samples")]
    public class HtmlBuilderController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
