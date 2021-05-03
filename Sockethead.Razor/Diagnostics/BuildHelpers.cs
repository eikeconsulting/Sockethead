using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sockethead.Razor.Helpers;
using System;
using System.Globalization;
using System.Reflection;

namespace Sylfph.Admin.HtmlHelpers
{
    /// <summary>
    /// Inspired by
    /// https://www.meziantou.net/getting-the-date-of-build-of-a-dotnet-assembly-at-runtime.htm
    /// </summary>
    public static class BuildHelpers
    {
        private static DateTime GetBuildDate(Assembly assembly)
        {
            const string BuildVersionMetadataPrefix = "+build";

            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute?.InformationalVersion == null)
                return default;

            string value = attribute.InformationalVersion;
            int index = value.IndexOf(BuildVersionMetadataPrefix);
            if (index <= 0)
                return default;

            value = value[(index + BuildVersionMetadataPrefix.Length)..];
            if (!DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return default;

            return result;
        }

        /// <summary>
        /// Get the project build time
        /// </summary>
        public static IHtmlContent BuildTime(this IHtmlHelper html)
            => html.ClientTime(GetBuildDate(Assembly.GetExecutingAssembly()));
    }
}
