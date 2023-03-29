using System;
using System.Collections.Generic;

namespace Sockethead.Common.Utilities
{
    /// <summary>
    ///  Offers a range of supporting functions that enhance the functionality of working with dates
    /// </summary>
    public static class DateUtils
    {
        /// <summary>
        /// Converts the specified day, month, and year to its equivalent integer value.
        /// For example, day:2/month:10/year:2022 would return 20221002.
        /// </summary>
        public static int GetDateValue(int day, int month, int year) => (year * 100 + month) * 100 + day;
        
        /// <summary>
        /// Extracts the day information from the given dateValue.
        /// For example, dateValue 20221002 would return 2 as day information
        /// </summary>
        public static int GetDay(int dateValue) => dateValue % 100;

        /// <summary>
        /// Extracts the month information from the given dateValue.
        /// For example, dateValue 20221002 would return 10 as month information
        /// </summary>
        public static int GetMonth(int dateValue) => (dateValue / 100) % 100;

        /// <summary>
        /// Extracts the year information from the given dateValue.
        /// For example, dateValue 20221002 would return 2022 as year information
        /// </summary>
        public static int GetYear(int dateValue) => dateValue / 10000;
    }
}