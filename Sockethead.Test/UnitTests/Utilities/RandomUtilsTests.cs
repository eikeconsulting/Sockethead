using System;
using Sockethead.Common.Utilities;
using Xunit;

namespace Sockethead.Test.UnitTests.Utilities
{
    public class RandomUtilsTests
    {
        [Fact]
        public void GetRandomInstanceByDate_ShouldReturnSameSequenceOfRandomNumbersForSameDate()
        {
            var date1 = new DateTime(2023, 3, 29);
            var date2 = new DateTime(2023, 3, 30);
        
            var random1 = RandomUtils.GetRandomInstanceByDate(date1);
            var random2 = RandomUtils.GetRandomInstanceByDate(date1);
            var random3 = RandomUtils.GetRandomInstanceByDate(date2);
            var random4 = RandomUtils.GetRandomInstanceByDate(date2);
        
            Assert.Equal(random1.Next(), random2.Next());
            Assert.Equal(random3.Next(), random4.Next());
            Assert.NotEqual(random1.Next(), random3.Next());
        }
    }
}