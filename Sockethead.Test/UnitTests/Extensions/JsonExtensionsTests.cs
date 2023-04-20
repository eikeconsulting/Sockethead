using Sockethead.Common.Extensions;
using Sockethead.Test.Common.Models;
using Xunit;

namespace Sockethead.Test.UnitTests.Extensions
{
    public class JsonExtensionsTests
    {
        [Fact]
        public void JsonSerializationAndDeserializationTests()
        {
            TestModel testModel = new TestModel("Sockethead", "Json Extensions");

            string json = testModel.ToJson();
            Assert.False(string.IsNullOrEmpty(json));

            TestModel deserializedModel = json.Deserialize<TestModel>();
            Assert.True(testModel.Property1 == deserializedModel.Property1);
            Assert.True(testModel.Property2 == deserializedModel.Property2);
        }
    }
}