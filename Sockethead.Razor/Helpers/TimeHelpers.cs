using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Globalization;

namespace Sockethead.Razor.Helpers
{
    public static class TimeHelpers
    {
        public static string ClientTimeHtml(DateTime? timestamp)
            => timestamp.HasValue
                ? $"<time>{timestamp.Value.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}</time>"
                : "N/A";

        public static IHtmlContent ClientTime(this IHtmlHelper html, DateTime? timestamp) 
            => html.Raw(ClientTimeHtml(timestamp));
    }
}
