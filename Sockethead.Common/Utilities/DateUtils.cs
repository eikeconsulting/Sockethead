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
        /// Returns a range of DateTime that starts with the startDate and ends with the endDate, inclusive of both endpoints.
        /// </summary>
        /// <param name="startDate">Start date and first element of enumerable</param>
        /// <param name="endDate">End date for enumerable</param>
        /// <returns>Enumerable for the days in the range</returns>
        public static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
        {
            for ( ; startDate <= endDate; startDate = startDate.AddDays(1))
                yield return startDate;
        }
    }
}