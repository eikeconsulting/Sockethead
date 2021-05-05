using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Sockethead.Razor.Helpers
{
    /// <summary>
    /// Enum Helper from SO:
    /// https://stackoverflow.com/a/13100409/910348
    /// </summary>
    public static class EnumHelper<T>
        where T : struct, Enum // This constraint requires C# 7.3 or later.
    {
        public static IList<T> GetValues(Enum value) 
            => value
                .GetType()
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(fi => (T)Enum.Parse(value.GetType(), fi.Name, false))
                .ToList();

        public static T Parse(string value) 
            => (T)Enum.Parse(typeof(T), value, true);

        public static IList<string> GetNames(Enum value) 
            => value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();

        public static IList<string> GetDisplayValues(Enum value) 
            => GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();

        private static string LookupResource(Type resourceManagerProvider, string resourceKey)
        {
            PropertyInfo pi = resourceManagerProvider.GetProperty(resourceKey,
                BindingFlags.Static | BindingFlags.Public, null, typeof(string),
                Array.Empty<Type>(), null);

            return pi == null 
                ? resourceKey // Fallback with the key name
                : (string)pi.GetMethod.Invoke(null, null);
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes[0].ResourceType != null)
                return LookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

            if (descriptionAttributes == null) 
                return string.Empty;

            return (descriptionAttributes.Length > 0) 
                ? descriptionAttributes[0].Name 
                : value.ToString();
        }

        public static string GetDisplayValueSafe(T value)
        {
            try
            {
                return GetDisplayValue(value);
            }
            catch
            {
                return value.ToString();
            }
        }
    }
}
