using FluentAssertions;
using Sockethead.ExtensionsAndUtilities.Extensions;
using Xunit;

namespace Sockethead.Test.UnitTests.Extensions
{
    public class JsonExtensionsTests
    {
        private class TestModel
        {
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }
        
        [Fact]
        public void JsonSerializationAndDeserializationTests()
        {
            TestModel testModel = new TestModel
            {
                Property1 = "Sockethead",
                Property2 = "Json Extensions"
            };

            string json = testModel.ToJson();
            Assert.False(string.IsNullOrEmpty(json));

            TestModel deserializedModel = json.Deserialize<TestModel>();
            Assert.True(testModel.Property1 == deserializedModel.Property1);
            Assert.True(testModel.Property2 == deserializedModel.Property2);
        }
    }
}