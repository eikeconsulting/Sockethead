using Microsoft.AspNetCore.Mvc;
using Sockethead.Razor.Alert.Extensions;
using Sockethead.Web.Data;
using Sockethead.Web.Data.Entities;
using System.Linq;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    [Area("Samples")]
    public class PagerController : Controller
    {
        private static IQueryable<SampleModel> SampleDataQuery => SampleData.SampleModels.AsQueryable();
        private static IQueryable<Movie> MovieQuery => SampleData.Movies.AsQueryable();

        [HttpGet]
        public IActionResult Movies1()
        {
            ViewData["Title"] = "Movies!";
            return View(MovieQuery.AsQueryable());
        }

        [HttpGet]
        public IActionResult Movies2()
        {
            ViewData["Title"] = "Movies!";
            return View(MovieQuery.AsQueryable());
        }

        [HttpGet]
        public IActionResult Movies3()
        {
            ViewData["Title"] = "Movies!";
            return View(MovieQuery.AsQueryable());
        }

        [HttpGet]
        public IActionResult TwoColumnGrid()
        {
            ViewData["Title"] = "TwoColumnGrid";
            return View(MovieQuery.AsQueryable());
        }

        [HttpGet]
        public IActionResult Sample1()
        {
            ViewData["Title"] = "Sample Data Example";
            return View(SampleDataQuery.AsQueryable());
        }

        [HttpGet]
        public IActionResult Sample2()
        {
            ViewData["Title"] = "Sample Data Example";
            return View(SampleDataQuery.AsQueryable());
        }

        [HttpGet]
        public IActionResult FormBuilder()
        {
            return View(SampleDataQuery.First());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult FormBuilder(SampleModel formData)
        {
            return View(SampleDataQuery.First()).Success($"Submitted form with {formData}");
        }

    }
}
