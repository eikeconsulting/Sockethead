using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sockethead.Razor.Forms
{
    public class FormOptions
    {
        public string ActionName { get; set; } = null;
        public string ControllerName { get; set; } = null;
        public FormMethod FormMethod { get; set; } = FormMethod.Post;
    }
     
    public static class SimpleFormExtensions
    {
        public static SimpleForm<T> SimpleForm<T>(this IHtmlHelper<T> html, T model, FormOptions options = default,
            string cssClass = "")
            where T : class => new(html: html, options: options, cssClass: cssClass);

        public static SimpleForm<T> SimpleForm<T>(this IHtmlHelper<T> html, T model, Action<FormOptions> optionsAction,
            string cssClass = "")
            where T : class
        {
            FormOptions options = new();
            optionsAction(options);
            return new(html: html, options: options, cssClass: cssClass);
        }
    }
}