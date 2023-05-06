using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sockethead.Razor.Alert.Extensions;
using Sockethead.Web.Data;
using Sockethead.Web.Data.Entities;
using System.Linq;
using Sockethead.Razor.Helpers;
using Sockethead.Razor.PRG;
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
        
        [HttpGet]
        public IActionResult CustomizeLayout()
        {
            return View(new UserProfile()).SetTitle("Customize Layout");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CustomizeLayout(UserProfile formData)
        {
            return View(formData).SetTitle("Customize Layout").Success($"Successfully submitted form data {formData}.");
        }

        [HttpGet, RestoreModelState]
        public IActionResult PostRedirectGet()
        {
            return View(new UserProfile { First = "BogusName", Last = "Smith" }).SetTitle("Post Redirect Get (PRG)");
        }

        [HttpPost, ValidateAntiForgeryToken, SaveModelState]
        public IActionResult PostRedirectGet(UserProfile formData)
        {
            RedirectToActionResult result = RedirectToAction(actionName: nameof(PostRedirectGet));

            if (formData.First == "BogusName")
                ModelState.AddModelError("First", "Sorry, we don't accept BogusName as a first name.");

            if (!ModelState.IsValid)
                return result;

            // handle form

            return result.Success($"Successfully submitted form data {formData}.");        
        }

        [HttpGet, RestoreModelState]
        public IActionResult CustomizeErrors()
        {
            return View(new UserProfile { First = "BogusName", Last = "Smith" }).SetTitle("Customize Error Messages");
        }

        [HttpPost, ValidateAntiForgeryToken, SaveModelState]
        public IActionResult CustomizeErrors(UserProfile formData)
        {
            RedirectToActionResult result = RedirectToAction(actionName: nameof(CustomizeErrors));

            if (formData.First == "BogusName")
                ModelState.AddModelError("First", "Sorry, we don't accept BogusName as a first name.");

            if (!ModelState.IsValid)
                return result;

            // handle form

            return result.Success($"Successfully submitted form data {formData}.");        
        }

        [HttpPost, ValidateAntiForgeryToken, SaveModelState]
        public IActionResult CustomizeErrors2(UserProfile formData)
        {
            RedirectToActionResult result = RedirectToAction(actionName: nameof(CustomizeErrors));

            if (formData.First == "BogusName")
                ModelState.AddModelError("First", "Sorry, we don't accept BogusName as a first name.");

            if (!ModelState.IsValid)
                return result.Error("My custom error message.");

            // handle form

            return result.Success($"Successfully submitted form data {formData}.");        
        }
    }
}
