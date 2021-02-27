using Microsoft.AspNetCore.Mvc;
using Sockethead.Web.Data;
using Sockethead.Web.Data.Entities;
using System.Linq;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    [Area("Samples")]
    public class SimpleGridController : Controller
    {
        private static IQueryable<SampleModel> SampleDataQuery => SampleData.SampleModels.AsQueryable();
        private static IQueryable<Movie> MovieQuery => SampleData.Movies.AsQueryable();

        public IActionResult Index() => RedirectToAction(actionName: nameof(Dashboard));

        public IActionResult Dashboard() => View();

        public IActionResult BasicUsage()
        {
            ViewData["Title"] = "Basic Usage";
            return View(MovieQuery.AsQueryable());
        }

        public IActionResult Playground()
        {
            ViewData["Title"] = "Playground";
            return View(MovieQuery.AsQueryable());
        }

        public IActionResult RowModifier()
        {
            ViewData["Title"] = "Row Modifier with CSS";
            return View(MovieQuery.AsQueryable());
        }

        [HttpGet]
        public IActionResult KitchenSink()
        {
            ViewData["Title"] = "Advanced SimpleGrid Example";
            return View(MovieQuery.AsQueryable());
        }

        [HttpGet]
        public IActionResult Sample1()
        {
            ViewData["Title"] = "Sample1";
            return View(SampleDataQuery.AsQueryable());
        }

        [HttpGet]
        public IActionResult Sample2()
        {
            ViewData["Title"] = "Sample2";
            return View(SampleDataQuery.AsQueryable());
        }

    }
}
