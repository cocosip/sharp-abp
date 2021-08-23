using SharpAbp.Abp.Core.Extensions;
using System.Linq;
using Xunit;

namespace SharpAbp.Abp.Core
{
    public class AbpDictionaryExtensionsTest
    {
        [Fact]
        public void TryAdd_Test()
        {
            var dict = new System.Collections.Generic.Dictionary<string, string>()
            {
                {"1","zhangsan" },
                {"2","lisi" }
            };

            Assert.True(dict.TryAdd<string, string>("3", "wangwu"));
            Assert.Equal(3, dict.Count);
            Assert.False(dict.TryAdd<string, string>("2", "lisi2"));
            Assert.Equal(3, dict.Count);
            Assert.Equal("lisi", dict["2"]);
        }

        [Fact]
        public void GetValueOrDefault_Test()
        {
            var dict = new System.Collections.Generic.Dictionary<string, string>()
            {
                {"123","xxx" },
                {"456","yyy" },
                {"789","" }
            };

            var v1 = dict.GetValueOrDefault("123", "33");
            Assert.Equal("xxx", v1);

            var v2 = dict.GetValueOrDefault("111", "xx");
            Assert.Equal("xx", v2);

            var v3 = dict.GetValueOrDefault("789", "x");
            Assert.Equal("", v3);
        }

        [Fact]
        public void Remove_Test()
        {
            var dict = new System.Collections.Generic.Dictionary<int, string>()
            {
                {1,"123" },
                {2,"456" },
                {3,"789" }
            };

            var r1 = dict.TryRemove(1, out string v1);
            Assert.True(r1);
            Assert.Equal("123", v1);

            var r2 = dict.TryRemove(4, out string v2);
            Assert.False(r2);
            Assert.Null(v2);

        }

        [Fact]
        public void ToDictionary_Test()
        {
            var dict = new System.Collections.Generic.Dictionary<int, string>()
            {
                {1,"11" },
                {2,"22" },
                {3,"33" }
            };

            var newDict = dict.ToDictionary<int, string, string, string>(x => x.ToString(), v => v);

            Assert.Equal(3, newDict.Count);
            Assert.True(newDict.ContainsKey("1"));
            Assert.True(newDict.ContainsKey("2"));


            var newDict2 = dict.ToDictionary(v => $"{v}_");
            Assert.Equal(3, newDict2.Count);
            Assert.Equal("11_", newDict2[1]);
            Assert.Equal("22_", newDict2[2]);
            Assert.Equal("33_", newDict2[3]);

            var keyValuePairs = dict.ToList();

            var newDict3 = keyValuePairs.AsDictionary();

            Assert.Equal(3, newDict3.Count);
            Assert.True(newDict3.ContainsKey(1));
            Assert.True(newDict3.ContainsKey(2));
        }

    }
}
