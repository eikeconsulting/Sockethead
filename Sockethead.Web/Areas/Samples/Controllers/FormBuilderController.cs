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

        [HttpGet]
        public IActionResult FormForModel() => View(new UserProfile
            {
                First = "John",
                Last = "Doe",
                JobTitle = "Software Engineer",
            });
                
        
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult FormForModel(UserProfile formData)
        {
            return View(formData).Success($"Submitted form with {formData}");
        }

    }
}
