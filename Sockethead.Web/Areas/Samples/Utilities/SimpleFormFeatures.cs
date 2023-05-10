using System.Collections.Generic;
using Sockethead.Web.Areas.Samples.ViewModels;

namespace Sockethead.Web.Areas.Samples.Utilities
{
    public static class SimpleFormFeatures
    {
        public static List<Feature> Features { get; } = new()
        {
            new()
            {
                Name = "Installation",
                Description = "Just install the NuGet package and go!",
            },
            new()
            {
                Url = "BasicUsage",
                Name = "Basic Usage",
                Description = "A minimal SimpleForm built manually.",
            },
            new()
            {
                Url = "HorizontalForm",
                Name = "Horizontal Form",
                Description = "Labels are on the left instead above fields, creating a more compact form.",
            },
            new()
            {
                Url = "AutoGenerateForm",
                Name = "Auto Generate Form",
                Description = "Have SimpleForm automatically generate the form from the decorated Model.",
            },
            new()
            {
                Url = "CustomizeLayout",
                Name = "Customize Layout",
                Description = "Customized the layout with rows and columns in your form.",
            },
            new()
            {
                Url = "PostRedirectGet",
                Name = "Post Redirect Get (PRG)",
                Description = "Implement the Post Redirect Get (PRG) pattern to avoid duplicate form submissions."
            },
            new()
            {
                Url = "CustomizeErrors",
                Name = "Customize Error Messages",
                Description = "Control how to display form validation errors."
            },
            new()
            {
                Url = "Prompt",
                Name = "Prompt for Placeholder Text",
                Description = "Use the Display Attribute 'Prompt' to set Placeholder text in a control."
            },
            new()
            {
                Url = "DataTypes",
                Name = "All Supported Data Types",
                Description = "A form that demonstrates all of the DataTypes supported by SimpleForm."
            },

            new()
            {
                Url = "KitchenSink",
                Name = "KitchenSink",
                Description = "A  SimpleForm, built automatically from a Model.",
            },
            new()
            {
                //Url = "SubmitButton",
                Name = "Submit Button",
                Description = "How to specify what text to render for the submit button and change the css class.",
            },
            new()
            {
                Name = "Email And Password",
                Description = "A sample SimpleForm using the Email and Password input types.",
            },
            new()
            {
                Name = "Date Time And Date",
                Description = "This demonstrates how to use the Date and DateTime input types.",
            },
            new()
            {
                Name = "Number Inputs",
                Description = "A sample SimpleForm which shows how to use the different number input types.",
            },
            new()
            {
                Name = "Text Area",
                Description = "SimpleForm which includes a text area.",
            },
            new()
            {
                Name = "Checkboxes and Radios",
                Description = "SimpleForm which includes how to use checkboxes and radios.",
            },
            new()
            {
                Name = "Drop Down List",
                Description = "Handling drop down lists.",
            },
            new()
            {
                Name = "Readonly And Disabled",
                Description = "Render a readonly or disabled field.",
            },
            new()
            {
                Name = "Append Custom Html",
                Description = "Append custom HTML to the SimpleForm.",
            },
            new()
            {
                Name = "File Upload",
                Description = "Handling file uploads.",
            },
        };
    }
}