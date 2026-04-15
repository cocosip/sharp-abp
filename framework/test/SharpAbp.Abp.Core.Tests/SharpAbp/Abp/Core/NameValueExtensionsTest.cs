using SharpAbp.Abp.Core.Extensions;
using System.Collections.Generic;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.Core
{
    public class NameValueExtensionsTest
    {
        [Fact]
        public void FindOrDefault_Should_Return_First_Matching_Item()
        {
            var values = new List<NameValue>
            {
                new NameValue("name", "first"),
                new NameValue("name", "second"),
                new NameValue("other", "value")
            };

            var result = values.FindOrDefault("name");

            Assert.NotNull(result);
            Assert.Equal("first", result.Value);
        }

        [Fact]
        public void FindOrDefault_Should_Return_Null_When_Item_Does_Not_Exist()
        {
            var values = new List<NameValue>
            {
                new NameValue("name", "first")
            };

            var result = values.FindOrDefault("missing");

            Assert.Null(result);
        }

        [Fact]
        public void FindValue_Should_Return_Empty_String_When_Item_Does_Not_Exist()
        {
            var values = new List<NameValue>
            {
                new NameValue("name", "first")
            };

            var result = values.FindValue("missing");

            Assert.Equal(string.Empty, result);
        }
    }
}
