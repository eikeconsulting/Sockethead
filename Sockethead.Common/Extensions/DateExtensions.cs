using System;
using System.Collections.Generic;

namespace Sockethead.Common.Extensions
{
    public static class DateExtensions
    {
        
        /// <summary>
        /// Returns a range of DateTime that starts with the startDate and ends with the endDate, inclusive of both endpoints.
        /// </summary>
        /// <param name="startDate">Start date and first element of enumerable</param>
        /// <param name="endDate">End date for enumerable</param>
        /// <returns>Enumerable for the days in the range</returns>
        public static IEnumerable<DateTime> GetDateRange(this DateTime startDate, DateTime endDate)
        {
            for ( ; startDate <= endDate; startDate = startDate.AddDays(1))
                yield return startDate;
        }
        
        /// <summary>
        /// Returns a range of DateTime objects that starts with startDate and ends with endDate, inclusive of both endpoints, incrementing by one month at a time.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetMonthRange(this DateTime startDate, DateTime endDate)
        {
            for ( ; startDate <= endDate; startDate = startDate.AddMonths(1))
                yield return startDate;
        }

        /// <summary>
        /// Returns a range of DateTime objects that starts with startDate and ends with endDate, inclusive of both endpoints, incrementing by one year at a time.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetYearRange(this DateTime startDate, DateTime endDate)
        {
            for ( ; startDate <= endDate; startDate = startDate.AddYears(1))
                yield return startDate;
        }
    }
}