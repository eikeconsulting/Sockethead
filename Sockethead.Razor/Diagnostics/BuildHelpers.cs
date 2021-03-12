using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sockethead.Razor.Helpers;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Sylfph.Admin.HtmlHelpers
{
    /// <summary>
    /// Inspired by
    /// https://www.meziantou.net/getting-the-date-of-build-of-a-dotnet-assembly-at-runtime.htm
    /// </summary>
    public static class BuildHelpers
    {
        private static DateTime GetLinkerTimestampUtc(Assembly assembly)
        {
            var location = assembly.Location;
            return GetLinkerTimestampUtc(location);
        }

        private static DateTime GetLinkerTimestampUtc(string filePath)
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;
            var bytes = new byte[2048];

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                file.Read(bytes, 0, bytes.Length);
            }

            var headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(bytes, headerPos + linkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddSeconds(secondsSince1970);
        }

        /// <summary>
        /// Doesn't work!
        /// </summary>
        public static IHtmlContent LinkerTime(this IHtmlHelper html)
            =>  html.ClientTime(GetLinkerTimestampUtc(Assembly.GetExecutingAssembly()));

        private static DateTime GetBuildDate(Assembly assembly)
        {
            const string BuildVersionMetadataPrefix = "+build";

            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute?.InformationalVersion == null)
                return default;

            var value = attribute.InformationalVersion;
            var index = value.IndexOf(BuildVersionMetadataPrefix);
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
