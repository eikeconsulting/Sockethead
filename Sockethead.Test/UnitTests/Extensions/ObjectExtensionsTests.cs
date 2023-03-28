using System.Collections.Generic;
using Sockethead.Common.Extensions;
using Sockethead.Test.Common.Models;
using Xunit;

namespace Sockethead.Test.UnitTests.Extensions
{
    public class ObjectExtensionsTests
    {
        private TestModel TestModel = new("Sockethead", "Object Extensions");
        
        [Fact]
        public void ObjectBase64ConversionTests()
        {
            string base64 = TestModel.ToBase64();
            Assert.False(string.IsNullOrEmpty(base64));

            TestModel convertedModel = base64.FromBase64<TestModel>();
            Assert.True(TestModel.Property1 == convertedModel.Property1);
            Assert.True(TestModel.Property2 == convertedModel.Property2);
        }

        [Fact]
        public void ObjectToDictionaryTests()
        {
            Dictionary<string, string> dictionary = TestModel.ToDictionary<string>();
            Assert.True(dictionary.Count == 2);
        }
    }
}