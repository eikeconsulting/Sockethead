using System.Collections.Generic;
using FluentAssertions;
using Sockethead.Common.Extensions;
using Xunit;

namespace Sockethead.Test.UnitTests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ToInt32OrDefaultTests()
        {
            string str = "5h";
            str.ToInt32OrDefault(defaultValue: 0).Should().Be(0);

            str = "5";
            str.ToInt32OrDefault(defaultValue: 0).Should().Be(5);
        }

        [Fact]
        public void TruncateTests()
        {
            string str = "Sockethead.Extensions";
            Assert.Equal("Sockethead", str.Truncate(10));
            
            Assert.Equal(str, str.Truncate(50));
        }

        [Fact]
        public void StripAccentsFromUnicodeCharactersTests()
        {
            string str = "Café au lait";
            Assert.Equal("Cafe au lait", str.StripAccentsFromUnicodeCharacters());
        }
        
        private enum WeekDay 
        {
            Monday, 
            Tuesday, 
            Wednesday
        }
        
        [Fact]
        public void ToEnumTests()
        {
            string weekDay = "Monday";
            WeekDay convertedEnum = weekDay.ToEnum(defaultValue: WeekDay.Wednesday);
            Assert.Equal(WeekDay.Monday, convertedEnum);

            weekDay = "null";
            convertedEnum = weekDay.ToEnum(defaultValue: WeekDay.Wednesday);
            Assert.Equal(WeekDay.Wednesday, convertedEnum);
        }
    }
}