using System;
using System.Collections.Generic;
using System.Linq;
using Sockethead.Common.Extensions;
using Xunit;

namespace Sockethead.Test.UnitTests.Extensions
{
    public class RandomExtensionsTests
    {
        [Fact]
        public void NextForTimesTests()
        {
            int maxValue = 100;
            int numberOfTimes = 5;
            Random rnd = new Random();
            List<int> list = rnd.NextForTimes(maxValue, numberOfTimes).ToList();
            Assert.True(list.Count == numberOfTimes);
        }
        
        [Fact]
        public void UniqueNextForTimesTests()
        {
            int maxValue = 100;
            int numberOfTimes = 5;
            Random rnd = new Random();
            List<int> list = rnd.UniqueNextForTimes(maxValue, numberOfTimes).ToList();
            Assert.True(list.Distinct().Count() == list.Count && list.Count == numberOfTimes);
        }
    }
}