using System.IO;
using Microsoft.AspNetCore.Http;
using Sockethead.Common.Extensions;
using Xunit;

namespace Sockethead.Test.UnitTests.Extensions
{
    public class FormFileExtensionsTests
    {
        [Fact]
        public void ToBase64_ReturnsBase64String()
        {
            string fileName = "test.jpg";
            byte[] fileContent = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            FormFile file = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, "test", fileName);

            string result = file.ToBase64();

            Assert.Equal("EjRWeA==", result);
        }
    }
}