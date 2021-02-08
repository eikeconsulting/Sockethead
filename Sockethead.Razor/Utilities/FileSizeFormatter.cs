using System;

namespace SSI.Common.Utilities
{
    public static class FileSizeFormatter
    {
        // Load all suffixes in an array  
        private static readonly string[] Suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        private static readonly int NumSuffixes = Suffixes.Length;

        /// <summary>
        /// Format a friendly display of file size
        /// </summary>
        public static string FormatSize(long bytes)
        {
            int counter = 1;
            decimal number = bytes;
            for (;  Math.Round(number / 1024) >= 1 && counter < NumSuffixes; counter++)
                number /= 1024;

            return $"{number:n1}{Suffixes[counter - 1]}";
        }
    }
}
