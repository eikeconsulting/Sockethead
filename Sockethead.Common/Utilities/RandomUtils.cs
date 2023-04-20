using System;

namespace Sockethead.Common.Utilities
{
    public class RandomUtils
    {
        /// <summary>
        /// Returns a new instance of the Random class, using the specified date as the seed value.
        /// The returned instance generates a same sequence of random numbers for a specified date.
        /// </summary>
        public static Random GetRandomInstanceByDate(DateTime date)
        {
            int seed = DateUtils.GetDateValue(date.Day, date.Month, date.Year);
            return new Random(seed);
        }
    }
}