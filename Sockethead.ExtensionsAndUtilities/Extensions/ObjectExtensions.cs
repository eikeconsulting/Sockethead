using System;
using System.Text;

namespace Sockethead.ExtensionsAndUtilities.Extensions
{
    /// <summary>
    /// Contains extension methods for the object.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts an object to its Base64-encoded string representation.
        /// </summary>
        public static string ToBase64(this object val) => Convert.ToBase64String(Encoding.UTF8.GetBytes(val.ToJson()));
        
        /// <summary>
        /// Converts a Base64-encoded string to an object of type T.
        /// </summary>
        public static T FromBase64<T>(this string val) => Encoding.UTF8.GetString(Convert.FromBase64String(val)).Deserialize<T>();
    }
}