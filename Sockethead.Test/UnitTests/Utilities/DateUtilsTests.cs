using Sockethead.Common.Utilities;
using Xunit;

namespace Sockethead.Test.UnitTests.Utilities
{
    public class DateUtilsTests
    {
        [Fact]
        public void GetDateValueTests()
        {
            Assert.Equal(20221102, DateUtils.GetDateValue(02,11,2022));
        }

        [Fact]
        public void GetDayTests()
        {
            Assert.Equal(02, DateUtils.GetDay(20221102));
            Assert.Equal(30, DateUtils.GetDay(20221130));
        }

        [Fact]
        public void GetMonthTests()
        {
            Assert.Equal(11, DateUtils.GetMonth(20221102));
            Assert.Equal(09, DateUtils.GetMonth(20220930));
        }

        [Fact]
        public void GetYearTests()
        {
            Assert.Equal(2022, DateUtils.GetYear(20221102));
            Assert.Equal(2023, DateUtils.GetYear(20230930));
        }
    }
}