using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sockethead.Razor.Alert.Extensions;
using Sockethead.Web.Data;
using Sockethead.Web.Data.Entities;
using System.Linq;
using Newtonsoft.Json;
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
            
            return View(viewName: name.Replace(" ", ""), model: model);
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
            return View(model: new UserProfile
            {
                UserId = Guid.NewGuid(),
                First = "John",
                Last = "Doe",
                JobTitle = "Software Developer",
                IsAdmin = true,
                Gender = Gender.Male,
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult BasicUsage(UserProfile formData)
        {
            return View(formData).Success($"Successfully submitted form data {formData}.");
        }

        [HttpGet]
        public IActionResult AutoGenerateForm()
        {
            return View(model: new UserProfile
            {
                UserId = Guid.NewGuid(),
                First = "John",
                Last = "Doe",
                JobTitle = "Software Developer",
                IsAdmin = true,
                Gender = Gender.Male,
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AutoGenerateForm(UserProfile formData)
        {
            if (formData.Last == "Doe")
            {
                ModelState.AddModelError("Last", "Sorry, we don't accept Doe as a last name.");
            }

            if (!ModelState.IsValid)
                return View(formData);

            return View(formData).Success($"Successfully submitted form data {formData}.");
        }
        
        [HttpGet]
        public IActionResult KitchenSink()
        {
            return View(SampleDataQuery.First());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult KitchenSink(SampleModel formData)
        {
            return View(formData).Success($"Successfully submitted form data {formData}.");
        }
        
        [HttpGet]
        public IActionResult CustomizeLayout()
        {
            return View(new UserProfile());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CustomizeLayout(UserProfile formData)
        {
            return View(formData).Success($"Successfully submitted form data {formData}.");
        }

        [HttpGet, RestoreModelState]
        public IActionResult PostRedirectGet()
        {
            return View(new UserProfile { First = "BogusName", Last = "Smith" });
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
            return View(new UserProfile { First = "BogusName", Last = "Smith" });
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
        
        [HttpGet]
        public IActionResult Prompt()
        {
            return View(new PromptExample());
        }

        [HttpGet]
        public IActionResult HorizontalForm()
        {
            return View(new UserProfile());
        }
        
        [HttpGet, RestoreModelState]
        public IActionResult DataTypes()
        {
            if (TempData.ContainsKey("DataTypesModel"))
            {
                ViewBag.Result = TempData["DataTypesModel"];
                TempData.Remove("DataTypesModel");
            }
            
            return View(new DataTypesModel());
        }

        [HttpPost, ValidateAntiForgeryToken, SaveModelState]
        public IActionResult DataTypes(DataTypesModel formData)
        {
            TempData["DataTypesModel"] = JsonConvert.SerializeObject(formData, Formatting.Indented);
            
            RedirectToActionResult result = RedirectToAction(actionName: nameof(DataTypes));
            
            return ModelState.IsValid
                ? result.Success("Successfully submitted form data. See the bottom of the page for serialized model data.")
                : result.Error("Error in model data.");
        }
    }
}
