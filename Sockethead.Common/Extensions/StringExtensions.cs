using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sockethead.Common.Extensions
{
    /// <summary>
    /// Contains extension methods for the string data type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Attempts to convert the string to an int. If the conversion is successful, the method returns the converted
        /// int value. If the conversion fails, the method returns a default value specified by the caller.
        /// </summary>
        public static int ToInt32OrDefault(this string value, int defaultValue = 0) =>
            int.TryParse(value, out int result) ? result : defaultValue;
        
        /// <summary>
        /// Returns a truncated version of a given string up to a specified maximum length
        /// </summary>
        public static string Truncate(this string value, int maxLength)
            => string.IsNullOrEmpty(value) ? value : value[..Math.Min(value.Length, maxLength)];
        
        /// <summary>
        /// Removes accents from Unicode characters in a given string.
        /// </summary>
        public static string StripAccentsFromUnicodeCharacters(this string input) =>
            string.Concat(input.Normalize(NormalizationForm.FormD).Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
        
        /// <summary>
        /// Converts the string representation to specified Enum.
        /// Returns defaultValue if value was not converted successfully.
        /// </summary>
        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue) where TEnum : struct
        {
            return !string.IsNullOrEmpty(value) && Enum.TryParse(value, true, out TEnum result)
                ? result
                : defaultValue;
        }
    }
}