using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sockethead.Common.Extensions
{
    /// <summary>
    /// Contains extension methods for the object.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Convert the object to a Dictionary
        /// Use only public properties of type T
        /// </summary>
        /// <param name="o">object to convert</param>
        /// <param name="useLowercaseKey">If true then converts the key to lowercase</param>
        /// <typeparam name="T">type for values</typeparam>
        /// <returns>Dictionary mapping property name to value of type T</returns>
        public static Dictionary<string, T> ToDictionary<T>(this object o, bool useLowercaseKey = false)
            => o.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(T))
                .ToDictionary(
                    prop => useLowercaseKey ? prop.Name.ToLower() : prop.Name,
                    prop => (T)prop.GetValue(o, null));
        
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