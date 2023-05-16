using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sockethead.Web.Areas.Samples.ViewModels;
using Sockethead.Web.Filters;

namespace Sockethead.Web.Areas.Samples.Extensions
{
    public static class ControllerExtensions
    {
        public static Feature SetSampleLinks(this Controller controller, string name = null)
        {
            List<Feature> features = (controller as IFeatureListController)?.Features;
            if (features == null)
                return null;
            
            string action = controller.RouteData.Values["action"]?.ToString();
            name ??= controller.Request.Query["name"];
            
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].Name != name && features[i].Url != action) 
                    continue;
                
                controller.ViewData["PrevFeature"] = i > 0 ? features[i - 1] : null;
                controller.ViewData["NextFeature"] = i + 1 < features.Count ? features[i + 1] : null;

                return features[i];
            }
            
            return null;
        }
    }
}