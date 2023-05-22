using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Sockethead.Razor.Alert.Extensions;
using Sockethead.Web.Data;
using Sockethead.Web.Data.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Sockethead.Razor.Forms;
using Sockethead.Razor.Helpers;
using Sockethead.Razor.PRG;
using Sockethead.Web.Areas.Samples.Utilities;
using Sockethead.Web.Areas.Samples.ViewModels;
using Sockethead.Web.Filters;
using System;

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
            return View(viewName: name.Replace(" ", ""), model: model);
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
        public IActionResult SelectionControls()
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
        public IActionResult SelectionControls(ExtendedUserProfile formData)
        {
            TempData["UserProfile"] = JsonConvert.SerializeObject(formData, Formatting.Indented);
            
            return this.SimpleFormHandler()
                .OnResult(RedirectToAction(nameof(SelectionControls)))
                .OnSuccess("Successfully submitted form data. See the bottom of the page for serialized model data.")
                .OnError("Error in model data.")
                .ProcessForm();
        }        

        private void PopulateSelectionControls()
        {
            ViewBag.CityList = ExtendedUserProfile.CityList;
            ViewBag.StateList = ExtendedUserProfile.StateList;
            ViewBag.CountryList = ExtendedUserProfile.CountryList;
            ViewBag.HobbyList = ExtendedUserProfile.HobbyList;
        }
        
        public IActionResult HorizontalForm()
        {
            PopulateSelectionControls();
           
            return View(new ExtendedUserProfile());
        }

        public IActionResult MixedLayoutForm()
        {
            PopulateSelectionControls();
            
            return View(new ExtendedUserProfile());
        }

        [HttpGet]
        public IActionResult RegisterEnums() => View(model: new ExtendedUserProfile());

        [HttpGet]
        public IActionResult CustomizeLayout1() => View(new ExtendedUserProfile());

        [HttpGet]
        public IActionResult CustomizeLayout2()
        {
            PopulateSelectionControls();
            
            return View(new ExtendedUserProfile());
        }

        [HttpGet]
        public IActionResult Prompt() => View(new PromptExample());

        
        //=============== TOSS =================
        
        /*
        [HttpGet]
        public IActionResult KitchenSink() => View(SampleDataQuery.First());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult KitchenSink(SampleModel formData) => 
            View(formData).Success($"Successfully submitted form data {formData}.");
        */

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
        public IActionResult DataTypes(DataTypesModel formData) =>
            this.SimpleFormHandler()
                .OnResult(() => 
                { 
                    TempData["DataTypesModel"] = JsonConvert.SerializeObject(formData, Formatting.Indented);
                    return RedirectToAction(actionName: nameof(DataTypes)); 
                })
                .OnSuccess("Successfully submitted form data. See the bottom of the page for serialized model data.")
                .OnError("Error in model data.")
                .ProcessForm();


        [HttpGet]
        public IActionResult FileUpload()
        {
            return View(new FileUploadViewModel());
        }
        
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> FileUpload(FileUploadViewModel formData)
        {
            IActionResult result = View(new FileUploadViewModel());

            ViewBag.Result = JsonConvert.SerializeObject(formData, Formatting.Indented);
            
            IFormFile image = formData.ImageFile;

            if (image.Length > 0)
            {
                await using Stream fileStream = image.OpenReadStream();
                byte[] bytes = new byte[image.Length];
                _ = await fileStream.ReadAsync(bytes.AsMemory(0, (int) image.Length));
                ViewBag.Image = $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
            }

            return result.Success("Boom");
        }
    }
}
