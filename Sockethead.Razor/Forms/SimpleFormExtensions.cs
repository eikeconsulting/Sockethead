using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sockethead.Razor.Forms
{
    public class FormOptions
    {
        public string ActionName { get; set; } = null;
        public string ControllerName { get; set; } = null;
        public FormMethod FormMethod { get; set; } = FormMethod.Post;

        public string CssClass { get; set; } = "";
    }

    public static class SimpleFormExtensions
    {
        public static SimpleForm<T> SimpleForm<T>(this IHtmlHelper<T> html, T model, Action<FormOptions> optionsAction)
            where T : class
        {
            FormOptions options = new();
            optionsAction(options);
            return new(html, optionsAction);
        }
    }
}