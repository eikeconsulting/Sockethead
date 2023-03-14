using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Sockethead.ExtensionsAndUtilities.Utilities;
using Xunit;

namespace Sockethead.Test.UnitTests.Utilities
{
    public class DateUtilsTests
    {
        [Fact]
        public void GetDateRangeTests()
        {
            const int days = 5;
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = startDate.AddDays(days);
            
            List<DateTime> dateRange = DateUtils.GetDateRange(startDate, endDate).ToList();
            Assert.NotNull(dateRange);
            dateRange.Should().HaveCount(days + 1);
            
            Assert.Equal(dateRange.First(), startDate);
            Assert.Equal(dateRange.Last(), endDate);
        }
    }
}