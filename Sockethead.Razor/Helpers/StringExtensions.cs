using System.Text.RegularExpressions;

namespace Sockethead.Razor.Helpers
{
    public static class StringExtensions
    {
        public static string PascalCaseAddSpaces(this string str)
            => Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {m.Value[1]}");

        public static string ToSentenceCase(this string str)
            => Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");


    }
}
