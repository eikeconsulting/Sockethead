using Microsoft.AspNetCore.Mvc;
using Sockethead.Web.Data;
using Sockethead.Web.Data.Entities;
using System.Linq;
using Sockethead.Razor.Helpers;
using Sockethead.Web.Areas.Samples.ViewModels;
using System.Collections.Generic;
using Sockethead.Razor.Alert.Extensions;
using Sockethead.Web.Areas.Samples.Helpers;
using Sockethead.Web.Areas.Samples.Utilities;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    [Area("Samples")]
    public class SimpleGridController : Controller
    {
        static List<Feature> Features { get; } = SimpleGridFeatures.Features;

        private static IQueryable<SampleModel> SampleDataQuery => SampleData.SampleModels.AsQueryable();
        private static IQueryable<Movie> MovieQuery => SampleData.Movies.AsQueryable();

        [HttpGet]
        public IActionResult Index() => RedirectToAction(actionName: nameof(Dashboard));

        [HttpGet]
        public IActionResult Dashboard() => View(Features.AsQueryable()).SetTitle("SimpleGrid");
        
        private string SetSampleLinks(string name)
        {
            Feature feature = FeatureHelper.SetSampleLinks(this, Features, name);
            return feature == null ? "Movies" : feature.Model;
        }

        [HttpGet]
        public IActionResult Sample(string name)
        {
            string modelName = SetSampleLinks(name);

            object model = modelName == "Movies" ? MovieQuery : (object)SampleDataQuery;

            return View(viewName: name.Replace(" ", ""), model: model).SetTitle(name);
        }

        [HttpGet]
        public IActionResult Form()
        {
            SetSampleLinks("Form");
            return View(MovieQuery).SetTitle("Form");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Form(string[] names)
        {
            SetSampleLinks("Form");
            return View(MovieQuery)
                .SetTitle("Form")
                .Success($"You selected: {string.Join(", ", names)}");
        }

        [HttpGet]
        public IActionResult Form2()
        {
            SetSampleLinks("Form2");
            return View(MovieQuery).SetTitle("Form2");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Form2(Movie[] movies)
        {
            SetSampleLinks("Form2");
            return View(movies.AsQueryable())
                .SetTitle("Form2")
                .Success($"Received: {movies.Length} movies");
        }

        [HttpGet]
        public PartialViewResult PartialGrid() => PartialView("_PartialGrid", MovieQuery);

        [HttpGet]
        public IActionResult Playground()
        {
            SetSampleLinks("Playground");
            return View(MovieQuery).SetTitle("Playground");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Playground(string[] names)
        {
            return View(MovieQuery.Where(movie => names.Contains(movie.Name)))
                .SetTitle("Playground Submit Results");
        }
    }
}
