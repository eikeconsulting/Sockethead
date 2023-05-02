using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sockethead.Razor.Alert.Extensions;
using Sockethead.Web.Data;
using Sockethead.Web.Data.Entities;
using System.Linq;
using Sockethead.Razor.Helpers;
using Sockethead.Web.Areas.Samples.Extensions;
using Sockethead.Web.Areas.Samples.Utilities;
using Sockethead.Web.Areas.Samples.ViewModels;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    [Area("Samples")]
    public class SimpleFormController : Controller
    {
        private static List<Feature> Features => SimpleFormFeatures.Features;

        private static IQueryable<SampleModel> SampleDataQuery => SampleData.SampleModels.AsQueryable();
        
        [HttpGet]
        public IActionResult Dashboard() => View(Features.AsQueryable()).SetTitle("SimpleForm");

        [HttpGet]
        public IActionResult Sample(string name)
        {
            this.SetSampleLinks(Features, name);
            SampleModel model = SampleDataQuery.First();
            model.View = name;
            
            return View(viewName: name.Replace(" ", ""), model: model).SetTitle(name);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Sample(SampleModel formData)
        {
            this.SetSampleLinks(Features, formData.View);
            return View(viewName: formData.View.Replace(" ", ""), formData)
                .Success($"Form submitted successfully.");
        }

        [HttpGet]
        public IActionResult BasicUsage()
        {
            this.SetSampleLinks(Features, "Basic Usage");
            return View(SampleDataQuery.First()).SetTitle("Basic Usage");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult BasicUsage(SampleModel formData)
        {
            this.SetSampleLinks(Features, "Basic Usage");
            return View(formData).Success($"Successfully submitted form data {formData}.");
        }

        [HttpGet]
        public IActionResult BuildForm()
        {
            this.SetSampleLinks(Features, "BuildForm");
            return View(SampleDataQuery.First()).SetTitle("Basic Usage");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult BuildForm(SampleModel formData)
        {
            this.SetSampleLinks(Features, "Build Form");
            return View(formData).Success($"Successfully submitted form data {formData}.");
        }
    }
}
