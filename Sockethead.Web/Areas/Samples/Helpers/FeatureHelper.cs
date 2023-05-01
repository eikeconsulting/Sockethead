using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sockethead.Web.Areas.Samples.ViewModels;

namespace Sockethead.Web.Areas.Samples.Helpers
{
    public static class FeatureHelper
    {
        public static Feature SetSampleLinks(Controller controller, IReadOnlyList<Feature> features, string name)
        {
            Feature feature = null;
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].Name != name) 
                    continue;
                
                feature = features[i];
                if (i > 0) controller.ViewData["PrevFeature"] = features[i - 1];
                if (i + 1 < features.Count) controller.ViewData["NextFeature"] = features[i + 1];
                break;
            }
            
            return feature;
        }
    }
}