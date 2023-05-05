using System;
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
using Sockethead.Web.Filters;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    [Area("Samples")]
    [SampleLinksActionFilter]
    public class SimpleFormController : Controller
    {
        private static List<Feature> Features => SimpleFormFeatures.Features;

        private static IQueryable<SampleModel> SampleDataQuery => SampleData.SampleModels.AsQueryable();
        
        [HttpGet]
        public IActionResult Dashboard() => View(Features.AsQueryable()).SetTitle("SimpleForm");

        [HttpGet]
        public IActionResult Sample(string name)
        {
            SampleModel model = SampleDataQuery.First();
            model.View = name;
            
            return View(viewName: name.Replace(" ", ""), model: model).SetTitle(name);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Sample(SampleModel formData)
        {
            return View(viewName: formData.View.Replace(" ", ""), formData)
                .Success($"Form submitted successfully.");
        }

        [HttpGet]
        public IActionResult BasicUsage()
        {
            return View(
                new UserProfile
                {
                    UserId = Guid.NewGuid(),
                    First = "John",
                    Last = "Doe",
                    JobTitle = "Software Developer",
                    IsAdmin = true,
                    Gender = Gender.Male,
                })
                .SetTitle("Basic Usage");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult BasicUsage(UserProfile formData)
        {
            return View(formData)
                .SetTitle("Basic Usage")
                .Success($"Successfully submitted form data {formData}.");
        }

        [HttpGet]
        public IActionResult AutoGenerateForm()
        {
            return View(
                    new UserProfile
                    {
                        UserId = Guid.NewGuid(),
                        First = "John",
                        Last = "Doe",
                        JobTitle = "Software Developer",
                        IsAdmin = true,
                        Gender = Gender.Male,
                    })
                .SetTitle("Auto Generate Form");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AutoGenerateForm(UserProfile formData)
        {
            if (formData.Last == "Doe")
            {
                ModelState.AddModelError("Last", "Sorry, we don't accept Doe as a last name.");
            }

            if (!ModelState.IsValid)
            {
                return View(formData).SetTitle("Auto Generate Form");
            }

            return View(formData)
                .SetTitle("Auto Generate Form")
                .Success($"Successfully submitted form data {formData}.");
        }
        
        [HttpGet]
        public IActionResult KitchenSink()
        {
            return View(SampleDataQuery.First()).SetTitle("Kitchen Sink");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult KitchenSink(SampleModel formData)
        {
            return View(formData).SetTitle("Kitchen Sink").Success($"Successfully submitted form data {formData}.");
        }
    }
}
