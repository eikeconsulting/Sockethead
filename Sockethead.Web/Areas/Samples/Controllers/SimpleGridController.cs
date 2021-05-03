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

        [HttpGet]
        public IActionResult Ajax()
        {
            ViewData["Title"] = "Ajax";
            return View();
        }

        [HttpGet]
        public PartialViewResult PartialGrid()
        {
            return PartialView("_PartialGrid", MovieQuery);
        }

        [HttpGet]
        public IActionResult Playground()
        {
            ViewData["Title"] = "Playground";
            return View(MovieQuery);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Playground(string[] names)
        {
            ViewData["Title"] = "Playground Submit Results";
            return View(MovieQuery.Where(movie => names.Contains(movie.Name)));
        }

        public IActionResult ColumnSelection()
        {
            ViewData["Title"] = "Column Selection";
            return View(MovieQuery);
        }

        public IActionResult RowModifier()
        {
            ViewData["Title"] = "Row Modifier with CSS";
            return View(MovieQuery);
        }

        [HttpGet]
        public IActionResult KitchenSink()
        {
            ViewData["Title"] = "Advanced SimpleGrid Example";
            return View(MovieQuery);
        }

        [HttpGet]
        public IActionResult Sample1()
        {
            ViewData["Title"] = "Sample1";
            return View(SampleDataQuery);
        }

        [HttpGet]
        public IActionResult Sample2()
        {
            ViewData["Title"] = "Sample2";
            return View(SampleDataQuery);
        }

    }
}
