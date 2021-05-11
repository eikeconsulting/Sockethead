using Microsoft.AspNetCore.Mvc;
using Sockethead.Web.Data;
using Sockethead.Web.Data.Entities;
using System.Linq;
using Sockethead.Razor.Helpers;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    [Area("Samples")]
    public class SimpleGridController : Controller
    {
        private static IQueryable<SampleModel> SampleDataQuery => SampleData.SampleModels.AsQueryable();
        private static IQueryable<Movie> MovieQuery => SampleData.Movies.AsQueryable();

        public IActionResult Index() => RedirectToAction(actionName: nameof(Dashboard));

        public IActionResult Dashboard() => View();

        [HttpGet]
        public IActionResult BasicUsage() => View(MovieQuery.AsQueryable()).SetTitle("Basic Usage");

        [HttpGet]
        public IActionResult ColumnSelection() => View(MovieQuery).SetTitle("Column Selection");

        [HttpGet]
        public IActionResult ColumnHeaders() => View(MovieQuery).SetTitle("Column Headers");

        [HttpGet]
        public IActionResult NoHeaders() => View(MovieQuery).SetTitle("No Headers");

        [HttpGet]
        public IActionResult ColumnDisplay() => View(MovieQuery).SetTitle("Column Display");

        [HttpGet]
        public IActionResult Links() => View(MovieQuery).SetTitle("Links");

        [HttpGet]
        public IActionResult Sorting() => View(MovieQuery).SetTitle("Sorting");

        [HttpGet]
        public IActionResult Pagination() => View(MovieQuery).SetTitle("Pagination");

        [HttpGet]
        public IActionResult EmbeddingGrids() => View(MovieQuery).SetTitle("Embedding Grids");

        [HttpGet]
        public IActionResult Ajax() => View().SetTitle("Ajax");

        [HttpGet]
        public PartialViewResult PartialGrid() => PartialView("_PartialGrid", MovieQuery);

        [HttpGet]
        public IActionResult Enum() => View();

        [HttpGet]
        public IActionResult Playground() => View(MovieQuery).SetTitle("Playground");

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Playground(string[] names) => View(MovieQuery.Where(movie => names.Contains(movie.Name))).SetTitle("Playground Submit Results");


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
