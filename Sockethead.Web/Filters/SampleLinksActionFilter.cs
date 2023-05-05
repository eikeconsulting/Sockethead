using Microsoft.AspNetCore.Mvc;
using Sockethead.Web.Areas.Samples.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sockethead.Web.Filters
{
    public class SampleLinksActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller is Controller controller)
            {
                controller.SetSampleLinks();
            }

            base.OnActionExecuting(filterContext);
        }        
    }
}