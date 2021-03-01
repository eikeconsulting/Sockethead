using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
            var oldWriter = htmlHelper.ViewContext.Writer;
            try
            {
                using var writer = new System.IO.StringWriter();

                htmlHelper.ViewContext.Writer = writer;

                htmlHelper.RenderPartial(
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
