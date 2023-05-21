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
                Description = "An introduction to SimpleForm.",
            },
            new()
            {
                Url = "ResolveRows",
                Name = "Resolve Rows",
                Description = "Let SimpleForm determine the best type for each row based on the decorated fields of the Model.",
            },
            new()
            {
                Url = "ResolveModel",
                Name = "Resolve Model",
                Description = "SimpleForm can build the entire form automatically via Reflection.",
            },
            new()
            {
                Url = "Options",
                Name = "Options",
                Description = "Control various aspects of SimpleForm via Options.",
            },
            new()
            {
                Url = "Buttons",
                Name = "Buttons",
                Description = "Add Submit and Link buttons to submit and reset a form.",
            },
            new()
            {
                Url = "PostRedirectGet",
                Name = "Post Redirect Get (PRG)",
                Description = "Implement the Post Redirect Get (PRG) pattern to avoid duplicate form submissions."
            },
            new()
            {
                Url = "HandleErrors",
                Name = "Handle Form Errors",
                Description = "Handle errors in a form with the PRG pattern."
            },
            new()
            {
                Url = "CustomizeErrors1",
                Name = "Customize Form Error Message",
                Description = "Override the errors displayed in a form after the postback."
            },
            new()
            {
                Url = "CustomizeErrors2",
                Name = "Custom Error Alert",
                Description = "Disable form level errors and display an error alert at the page level."
            },
            new()
            {
                Url = "FormHandler",
                Name = "SimpleForm Handler",
                Description = "Use SimpleFormHandler in the controller to handle forms (any form)."
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
            new()
            {
                Url = "FormSize",
                Name = "Form Size",
                Description = "Control the size of the form rows.",
            },
        };
    }
}