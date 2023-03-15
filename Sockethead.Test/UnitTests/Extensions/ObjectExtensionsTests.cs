using Sockethead.ExtensionsAndUtilities.Extensions;
using Sockethead.Test.Common.Models;
using Xunit;

namespace Sockethead.Test.UnitTests.Extensions
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void ObjectBase64ConversionTests()
        {
            TestModel testModel = new TestModel("Sockethead", "Object Extensions");
            string base64 = testModel.ToBase64();
            Assert.False(string.IsNullOrEmpty(base64));

            TestModel convertedModel = base64.FromBase64<TestModel>();
            Assert.True(testModel.Property1 == convertedModel.Property1);
            Assert.True(testModel.Property2 == convertedModel.Property2);
        }
    }
}