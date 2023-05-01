using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sockethead.Web.Areas.Samples.ViewModels;

namespace Sockethead.Web.Areas.Samples.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Sets the Prev and Next links for the sample pages and returns the current feature. 
        /// </summary>
        public static Feature SetSampleLinks(this Controller controller, IReadOnlyList<Feature> features, string name)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].Name != name) 
                    continue;
                
                controller.ViewData["PrevFeature"] = i > 0 ? features[i - 1] : null;
                controller.ViewData["NextFeature"] = i + 1 < features.Count ? features[i + 1] : null;

                return features[i];
            }
            
            return null;
        }
    }
}