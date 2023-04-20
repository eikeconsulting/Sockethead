using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Sockethead.Common.Extensions;
using Xunit;

namespace Sockethead.Test.UnitTests.Extensions
{
    public class DateExtensionsTests
    {
        [Fact]
        public void GetDateRangeTests()
        {
            const int days = 5;
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = startDate.AddDays(days);
            
            List<DateTime> dateRange = startDate.GetDateRange(endDate).ToList();
            Assert.NotNull(dateRange);
            dateRange.Should().HaveCount(days + 1);
            
            Assert.Equal(dateRange.First(), startDate);
            Assert.Equal(dateRange.Last(), endDate);
        }
        
        [Fact]
        public void GetMonthRangeTests()
        {
            const int months = 5;
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = startDate.AddMonths(months);
            
            List<DateTime> monthRange = startDate.GetMonthRange(endDate).ToList();
            Assert.NotNull(monthRange);
            monthRange.Should().HaveCount(months + 1);
            
            Assert.Equal(monthRange.First(), startDate);
            Assert.Equal(monthRange.Last(), endDate);
        }
        
        [Fact]
        public void GetYearRangeTests()
        {
            const int years = 5;
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = startDate.AddYears(years);
            
            List<DateTime> yearRange = startDate.GetYearRange(endDate).ToList();
            Assert.NotNull(yearRange);
            yearRange.Should().HaveCount(years + 1);
            
            Assert.Equal(yearRange.First(), startDate);
            Assert.Equal(yearRange.Last(), endDate);
        }
    }
}