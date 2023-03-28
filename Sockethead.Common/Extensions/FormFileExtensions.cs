using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Sockethead.Common.Extensions
{
    public static class FormFileExtensions
    {
        /// <summary>
        /// Converts the file content to its Base64-encoded string representation.
        /// </summary>
        public static string ToBase64(this IFormFile file)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            byte[] fileBytes = ms.ToArray();
            return Convert.ToBase64String(fileBytes);
        }
    }
}