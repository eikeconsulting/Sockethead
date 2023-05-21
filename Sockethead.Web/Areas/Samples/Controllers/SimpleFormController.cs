using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sockethead.Razor.Alert.Extensions;
using Sockethead.Web.Data;
using Sockethead.Web.Data.Entities;
using System.Linq;
using Newtonsoft.Json;
using Sockethead.Razor.Forms;
using Sockethead.Razor.Helpers;
using Sockethead.Razor.PRG;
using Sockethead.Web.Areas.Samples.Utilities;
using Sockethead.Web.Areas.Samples.ViewModels;
using Sockethead.Web.Filters;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    [Area("Samples")]
    [SampleLinksActionFilter]
    public class SimpleFormController : Controller, IFeatureListController
    {
        public List<Feature> Features => SimpleFormFeatures.Features;

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
        public IActionResult BasicUsage() => View(model: new UserProfileNoAttributes());

        [HttpGet]
        public IActionResult ResolveRows() => View(model: new UserProfile());

        [HttpGet]
        public IActionResult ResolveModel() => View(model: new UserProfile());

        [HttpGet]
        public IActionResult Options() => View(model: new UserProfile());
        
        [HttpGet]
        public IActionResult FormSize() => View(model: new UserProfile());
        
        [HttpGet]
        public IActionResult Buttons() => View(model: new UserProfile());

        [HttpPost]
        public IActionResult Buttons(UserProfile formData)
        {
            return View(formData)
                .Success($"Submitted {formData.First} {formData.Last}.");
        }

        [HttpGet, RestoreModelState]
        public IActionResult PostRedirectGet() => View(new UserProfile());

        [HttpPost, ValidateAntiForgeryToken, SaveModelState]
        public IActionResult PostRedirectGet(UserProfile formData)
        {
            return RedirectToAction(actionName: nameof(PostRedirectGet))
                .Success($"Successfully submitted form data.");        
        }

        [HttpGet, RestoreModelState]
        public IActionResult HandleErrors() => View(new UserProfile { First = "BogusName", Last = "Smith" });

        [HttpPost, ValidateAntiForgeryToken, SaveModelState]
        public IActionResult HandleErrors(UserProfile formData)
        {
            RedirectToActionResult result = RedirectToAction(actionName: nameof(HandleErrors));

            if (formData.First == "BogusName")
                ModelState.AddModelError(
                    key: "First", 
                    errorMessage: "Sorry, we don't accept BogusName as a first name.");

            if (!ModelState.IsValid)
                return result;

            // handle form

            return result.Success($"Successfully submitted form data.");        
        }
        
        [HttpGet, RestoreModelState]
        public IActionResult CustomizeErrors1() => View(new UserProfile {First = "BogusName", Last = "Smith"});

        [HttpGet, RestoreModelState]
        public IActionResult CustomizeErrors2() => View(new UserProfile {First = "BogusName", Last = "Smith"});

        [HttpPost, ValidateAntiForgeryToken, SaveModelState]
        public IActionResult CustomizeErrors(UserProfile formData)
        {
            string referrer = HttpContext.Request.Headers["Referer"];
            IActionResult result = Redirect(referrer);

            if (formData.First == "BogusName")
                ModelState.AddModelError("First", "Sorry, we don't accept BogusName as a first name.");

            if (!ModelState.IsValid)
            {
                if (referrer.Contains("CustomizeErrors2"))
                    result = result.Error("My custom error message.");
                return result;
            }

            /* do actual form processing logic here */

            return result.Success($"Successfully submitted form data.");        
        }

        [HttpGet, RestoreModelState]
        public IActionResult FormHandler() => View(new UserProfile {First = "BogusName", Last = "Smith"});

        [HttpPost, ValidateAntiForgeryToken, SaveModelState]
        public IActionResult FormHandler(UserProfile formData) =>
            this.SimpleFormHandler()
                .OnResult(RedirectToAction(nameof(FormHandler)))
                .OnError("My custom error message.")
                .OnSuccess($"Successfully submitted form data.")
                .Validate(v => v
                    .For("First")
                    .Must(formData.First != "BogusName")
                    .Message("Sorry, we don't accept BogusName as a first name."))
                .ProcessForm(() =>
                {
                    /* do actual form processing logic here */
                });
        
        [HttpGet, RestoreModelState]
        public IActionResult HorizontalForm()
        {
            ExtendedUserProfile model = new();
            
            if (TempData.ContainsKey("UserProfile"))
            {
                string json = TempData["UserProfile"].ToString();
                if (json != null)
                {
                    model = JsonConvert.DeserializeObject<ExtendedUserProfile>(json);
                    ViewBag.Result = json;
                }
                TempData.Remove("UserProfile");
            }

            ViewBag.CityList = ExtendedUserProfile.CityList;
            ViewBag.StateList = ExtendedUserProfile.StateList;
            ViewBag.CountryList = ExtendedUserProfile.CountryList;
            ViewBag.HobbyList = ExtendedUserProfile.HobbyList;
            
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, SaveModelState]
        public IActionResult HorizontalForm(ExtendedUserProfile formData)
        {
            TempData["UserProfile"] = JsonConvert.SerializeObject(formData, Formatting.Indented);
            
            RedirectToActionResult result = RedirectToAction(actionName: nameof(HorizontalForm));
            
            return ModelState.IsValid
                ? result.Success("Successfully submitted form data. See the bottom of the page for serialized model data.")
                : result.Error("Error in model data.");
        }        

        //=============== TOSS =================
        
        [HttpGet]
        public IActionResult AutoGenerateForm() => View(model: new UserProfile());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AutoGenerateForm(UserProfile formData)
        {
            if (formData.First == "BogusName")
                ModelState.AddModelError("First", "Sorry, we don't accept BogusName as a first name.");

            if (!ModelState.IsValid)
                return View(formData)
                    .Error("Error in form submission.");

            /* do actual form processing logic here */
            
            return View(formData)
                .Success($"Successfully submitted form data.");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AutoGenerateForm2(UserProfile formData) =>
            this.SimpleFormHandler()
                .OnResult(() => View(viewName: nameof(AutoGenerateForm), model: formData))
                .OnError(result => result.Error("Error in form submission."))
                .OnSuccess(result => result.Success($"Successfully submitted form data {formData}."))
                .ProcessForm(() =>
                {
                    if (formData.Last == "Doe")
                    {
                        ModelState.AddModelError("Last", "Sorry, we don't accept Doe as a last name.");
                        //return;
                    }

                    /* do actual form processing logic here */
                });


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AutoGenerateForm3(UserProfile formData) =>
            this.SimpleFormHandler()
                .OnResult(View(viewName: nameof(AutoGenerateForm), model: formData))
                .OnError("Error in form submission.")
                .OnSuccess($"Successfully submitted form data {formData}.")
                .Validate(v => v
                    .For("Last")
                    .Must(formData.Last != "Doe")
                    .Message("Sorry, we don't accept Doe as a last name."))
                .ProcessForm(() =>
                {
                    /* do actual form processing logic here */
                });
        
        [HttpGet]
        public IActionResult KitchenSink() => View(SampleDataQuery.First());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult KitchenSink(SampleModel formData) => View(formData).Success($"Successfully submitted form data {formData}.");

        [HttpGet]
        public IActionResult CustomizeLayout() => View(new ExtendedUserProfile());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CustomizeLayout(ExtendedUserProfile formData) => View(formData).Success($"Successfully submitted form data {formData}.");

        [HttpGet]
        public IActionResult Prompt() => View(new PromptExample());
        




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
