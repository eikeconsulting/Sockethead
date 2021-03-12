using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Sockethead.Razor.Diagnostics
{
    public class TimeTrackerAttribute : ActionFilterAttribute
    {
        private const string TimeTrackerKey = "TimeTrackerKey";

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Items[TimeTrackerKey] = Stopwatch.StartNew();
            base.OnResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
            // do something here?
        }
    }
}
