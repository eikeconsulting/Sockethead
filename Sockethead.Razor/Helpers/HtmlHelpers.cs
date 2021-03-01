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

        public static async Task<string> RenderPartialToStringAsync(
            this IHtmlHelper htmlHelper,
            string partialViewName,
            object model,
            ViewDataDictionary viewData = null)
        {
            var oldWriter = htmlHelper.ViewContext.Writer;
            try
            {
                using var writer = new System.IO.StringWriter();

                htmlHelper.ViewContext.Writer = writer;

                await htmlHelper.RenderPartialAsync(
                    partialViewName: partialViewName,
                    model: model,
                    viewData: viewData);

                return writer.GetStringBuilder().ToString();
            }
            finally
            {
                htmlHelper.ViewContext.Writer = oldWriter;
            }
        }
    }
}
