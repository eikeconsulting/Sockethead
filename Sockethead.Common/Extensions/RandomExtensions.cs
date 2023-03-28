using System;
using System.Collections.Generic;

namespace Sockethead.Common.Extensions
{
    /// <summary>
    /// Provides a set of extension methods that add additional functionality to Random
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns an IEnumerable of random integers up to a maximum value, with the number of integers determined by a specified count.
        /// </summary>
        public static IEnumerable<int> NextForTimes(this Random random, int maxValue, int numberOfTimes)
        {
            for (int i = 0; i < numberOfTimes; i++)
                yield return random.Next(maxValue);
        }

        /// <summary>
        /// Returns an IEnumerable of unique random integers up to a maximum value, with the number of integers determined by a specified count.
        /// </summary>
        public static IEnumerable<int> UniqueNextForTimes(this Random random, int maxValue, int numberOfTimes)
        {
            if (maxValue < numberOfTimes)
                throw new ArgumentException("maxValue is less than numberOfTimes.");

            HashSet<int> values = new();

            for (int i = 0; i < numberOfTimes;)
            {
                int value = random.Next(maxValue);
                if(values.Add(value))
                    i += 1;
            }

            return values;
        }
        
    }
}