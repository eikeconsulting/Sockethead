using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sockethead.Razor.Forms
{
    public class FormOptions
    {
        /// <summary>
        /// Controller action name for the form post
        /// </summary>
        public string ActionName { get; set; }
        
        /// <summary>
        /// Controller name for the form post
        /// </summary>
        public string ControllerName { get; set; }
        
        /// <summary>
        /// Form method for the form post, defaults to POST
        /// </summary>
        public FormMethod FormMethod { get; set; } = FormMethod.Post;
        
        /// <summary>
        /// Show errors when the form is submitted
        /// </summary>
        public bool ShowError { get; set; } = true;
        
        /// <summary>
        /// The (global) error message when an error occurs upon form submission
        /// </summary>
        public string ErrorMessage { get; set; } = "Oops!  We had a problem processing the form.";
        
        /// <summary>
        /// Whether to show individual error messages for each field globally
        /// (they will still always appear below each field in the form)
        /// </summary>
        public bool ShowValidationSummary { get; set; } = true;
        
        public bool HideLabels { get; set; } = false;
    }
}
