using System;
using System.Collections.Generic;
using Sockethead.Common.Extensions;
using Xunit;

namespace Sockethead.Test.UnitTests.Extensions
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public void EmptyIfNullTests()
        {
            IEnumerable<string> list = null;
            list = list.EmptyIfNull();
            Assert.NotNull(list);
        }
    }
}