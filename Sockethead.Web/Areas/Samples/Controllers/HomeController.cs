using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sockethead.Razor.Alert.Extensions;
using Sockethead.Razor.Diagnostics;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    [Area("Samples")]
    [TimeTracker]
    public class HomeController : Controller
    {
        public IActionResult Dashboard() => View();
    }
}
