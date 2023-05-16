using System.Collections.Generic;
using Sockethead.Web.Areas.Samples.ViewModels;

namespace Sockethead.Web.Filters
{
    public interface IFeatureListController
    {
        List<Feature> Features { get; }
    }
}