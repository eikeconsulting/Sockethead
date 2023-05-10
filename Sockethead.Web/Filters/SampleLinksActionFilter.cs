using Microsoft.AspNetCore.Mvc;
using Sockethead.Web.Areas.Samples.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Sockethead.Razor.Helpers;
using Sockethead.Web.Areas.Samples.ViewModels;

namespace Sockethead.Web.Filters
{
    public class SampleLinksActionFilter : ActionFilterAttribute
    {
        private Feature _feature;
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller is Controller controller)
                _feature = controller.SetSampleLinks();

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (_feature != null && context.Result is ViewResult viewResult)
                viewResult.SetTitle(_feature.Name);

            base.OnActionExecuted(context);
        }
    }
}