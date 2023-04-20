using System.Collections.Generic;
using Sockethead.Common.Extensions;
using Sockethead.Test.Common.Models;
using Xunit;

namespace Sockethead.Test.UnitTests.Extensions
{
    public class DictionaryExtensionsTests
    {
        [Fact]
        public void Accumulate_AddValueToDictionary_WhenKeyNotExists()
        {
            Dictionary<string, int> dictionary = new();

            dictionary.Accumulate("key", 10, (acc, v) => acc + v);

            Assert.Single(dictionary);
            Assert.Equal(10, dictionary["key"]);
        }

        [Fact]
        public void Accumulate_AccumulateValueToDictionary_WhenKeyExists()
        {
            Dictionary<string, int> dictionary = new() { { "key", 10 } };

            dictionary.Accumulate("key", 20, (acc, v) => acc + v);

            Assert.Single(dictionary);
            Assert.Equal(30, dictionary["key"]);
        }
        
        [Fact]
        public void AccumulateSum_AddValueToDictionary_WhenKeyExists()
        {
            Dictionary<string, decimal> dictionary = new() { { "key", 10 } };

            dictionary.AccumulateSum("key", 20);

            Assert.Single(dictionary);
            Assert.Equal(30m, dictionary["key"]);
        }
        
        [Fact]
        public void ToObject_ConvertsDictionaryToObject_Successfully()
        {
            var dictionary = new Dictionary<string, object>()
            {
                { "Property1", "Hello" },
                { "Property2", "World!" }
            };

            TestModel testModelObject = dictionary.ToObject<TestModel>();

            Assert.Equal("Hello", testModelObject.Property1);
            Assert.Equal("World!", testModelObject.Property2);
        }
    }
}