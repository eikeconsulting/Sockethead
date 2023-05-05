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
                Url = "BasicUsage",
                Name = "Basic Usage",
                Description = "A minimal SimpleForm built manually.",
            },
            new Feature
            {
                Url = "AutoGenerateForm",
                Name = "Auto Generate Form",
                Description = "Have SimpleForm automatically generate the form from the decorated Model.",
            },
            new Feature
            {
                Url = "KitchenSink",
                Name = "KitchenSink",
                Description = "A  SimpleForm, built automatically from a Model.",
            },
            new Feature
            {
                //Url = "SubmitButton",
                Name = "Submit Button",
                Description = "How to specify what text to render for the submit button and change the css class.",
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