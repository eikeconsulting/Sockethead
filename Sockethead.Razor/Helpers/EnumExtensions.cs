using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Sockethead.Razor.Helpers
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets an Enum's:
        /// 1. Display.Name if it exists
        /// 2. EnumName converted to string
        /// 3. Integral value if the Enum value is out of range
        /// </summary>
        public static string GetDisplayName(this Enum value)
        {
            string s = value.ToString();
            try
            {
                var type = value.GetType();
                return type?
                    .GetMember(s)?
                    .First()?
                    .GetCustomAttribute<DisplayAttribute>()?
                    .Name
                    ?? Enum.GetName(type, value);
            }
            catch
            {
                return s;
            }
        }
    }
}
