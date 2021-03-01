using Microsoft.AspNetCore.Mvc;
using Sockethead.Razor.Alert.Extensions;
using Sockethead.Web.Data;
using Sockethead.Web.Data.Entities;
using System.Linq;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    [Area("Samples")]
    public class FormBuilderController : Controller
    {
        private static IQueryable<SampleModel> SampleDataQuery => SampleData.SampleModels.AsQueryable();
        private static IQueryable<Movie> MovieQuery => SampleData.Movies.AsQueryable();


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
