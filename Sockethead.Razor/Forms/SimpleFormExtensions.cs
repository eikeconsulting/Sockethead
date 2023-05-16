using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sockethead.Razor.Forms
{
    public static class SimpleFormExtensions
    {
        public static SimpleForm<T> SimpleForm<T>(
            this IHtmlHelper<T> html, T model, 
            Action<FormOptions> optionsAction = null) where T : class => 
            new(html, optionsAction);
        
        public static SimpleFormHandler SimpleFormHandler(this Controller controller) => 
            new(controller.ModelState);
    }
}