using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sockethead.Razor.Areas.Sockethead.Controllers
{
    [Area("Sockethead")]
    public class SimpleGridController : Controller
    {
        public IActionResult Ping() => Content("Hello World!");


    }
}
