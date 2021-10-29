using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Diagnostics;

namespace Sockethead.Razor.Diagnostics
{
    internal static class TimeTracker
    {
        private const string TimeTrackerKey = "TimeTrackerKey";

        public static void Start(ViewDataDictionary viewData)
        {
            viewData[TimeTrackerKey] = Stopwatch.StartNew();
        }

        public static TimeSpan Stop(ViewDataDictionary viewData)
        {
            return viewData[TimeTrackerKey] is Stopwatch sw
                ? sw.Elapsed
                : TimeSpan.Zero;
        }
    }

    public class TimeTrackerAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var ctrl = context.Controller as Controller;
            TimeTracker.Start(ctrl.ViewData);
            base.OnResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
        }
    }

    public static class TimeTrackerExtensions
    {
        public static TimeSpan ElaspsedPageRenderTime(this IHtmlHelper html)
            => TimeTracker.Stop(html.ViewData);

        public static string ElaspsedPageRenderTimeMs(this IHtmlHelper html)
            => html
                .ElaspsedPageRenderTime()
                .TotalMilliseconds
                .ToString("N0");
    }
}
