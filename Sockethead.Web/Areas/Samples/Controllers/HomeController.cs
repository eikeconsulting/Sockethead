using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sockethead.Razor.Alert.Extensions;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    [Area("Samples")]
    public class HomeController : Controller
    {
        public IActionResult Dashboard() => View().Success("Welcome to Sockethead!");
    }
}
