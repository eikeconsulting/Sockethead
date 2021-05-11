using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sockethead.Razor.Helpers
{
    /// <summary>
    /// https://www.adamrussell.com/asp-net-core-section-scripts-in-a-partial-view/
    /// </summary>
    public static class HtmlHelperExtensions
    {
        private const string _partialViewScriptItemPrefix = "scripts_";
        
        public static IHtmlContent PartialSectionScripts(this IHtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items[_partialViewScriptItemPrefix + Guid.NewGuid()] = template;
            return new HtmlContentBuilder();
        }

        public static IHtmlContent RenderPartialSectionScripts(this IHtmlHelper htmlHelper)
        {
            var partialSectionScripts = htmlHelper.ViewContext.HttpContext.Items.Keys
                .Where(k => Regex.IsMatch(
                    k.ToString(),
                    "^" + _partialViewScriptItemPrefix + "([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$"));

            var contentBuilder = new HtmlContentBuilder();
            foreach (var key in partialSectionScripts)
            {
                if (htmlHelper.ViewContext.HttpContext.Items[key] is Func<object, HelperResult> template)
                {
                    var writer = new System.IO.StringWriter();
                    template(null).WriteTo(writer, HtmlEncoder.Default);
                    contentBuilder.AppendHtml(writer.ToString());
                }
            }
            return contentBuilder;
        }

        public static void SetTitle(this IHtmlHelper html, string title)
        {
            html.ViewContext.ViewData["Title"] = title;
        }

    }
}
