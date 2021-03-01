using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;

namespace Sockethead.Razor.Helpers
{
    public static class HtmlHelpers
    {
        public static string RenderPartialToString(
            this IHtmlHelper htmlHelper, 
            string partialViewName, 
            object model, 
            ViewDataDictionary viewData = null)
        {
            return htmlHelper.RenderPartialToStringAsync(partialViewName, model, viewData).Result;
        }

        /// <summary>
        /// Render a partial view to a string by temporarly 
        /// replacing the view's writer (kludge!)
        /// </summary>
        public static async Task<string> RenderPartialToStringAsync(
            this IHtmlHelper htmlHelper,
            string partialViewName,
            object model,
            ViewDataDictionary viewData = null)
        {
            var viewWriter = htmlHelper.ViewContext.Writer;
            try
            {
                using var stringWriter = new System.IO.StringWriter();

                htmlHelper.ViewContext.Writer = stringWriter;

                await htmlHelper.RenderPartialAsync(
                    partialViewName: partialViewName,
                    model: model,
                    viewData: viewData);

                return stringWriter.GetStringBuilder().ToString();
            }
            finally
            {
                // restore the original writer back to the View
                htmlHelper.ViewContext.Writer = viewWriter;
            }
        }
    }
}
