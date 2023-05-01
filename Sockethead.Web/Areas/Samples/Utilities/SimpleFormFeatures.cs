using System.Collections.Generic;
using Sockethead.Web.Areas.Samples.ViewModels;

namespace Sockethead.Web.Areas.Samples.Utilities
{
    public static class SimpleFormFeatures
    {
        public static List<Feature> Features { get; } = new()
        {
            new Feature
            {
                Name = "Installation",
                Description = "Just install the NuGet package and go!",
            },
            new Feature
            {
                Name = "Basic Usage",
                Description = "A minimal SimpleForm.",
            },
            new Feature
            {
                Name = "Email And Password",
                Description = "A sample SimpleForm using the Email and Password input types.",
            },
            new Feature
            {
                Name = "Date Time And Date",
                Description = "This demonstrates how to use the Date and DateTime input types.",
            },
            new Feature
            {
                Name = "Number Inputs",
                Description = "A sample SimpleForm which shows how to use the different number input types.",
            },
            new Feature
            {
                Name = "Text Area",
                Description = "SimpleForm which includes a text area.",
            },
            new Feature
            {
                Name = "Checkboxes and Radios",
                Description = "SimpleForm which includes how to use checkboxes and radios.",
            },
            new Feature
            {
                Name = "Drop Down List",
                Description = "Handling drop down lists.",
            },
            new Feature
            {
                Name = "Readonly And Disabled",
                Description = "Render a readonly or disabled field.",
            },
            new Feature
            {
                Name = "Append Custom Html",
                Description = "Append custom HTML to the SimpleForm.",
            },
            new Feature
            {
                Name = "File Upload",
                Description = "Handling file uploads.",
            },
        };
    }
}