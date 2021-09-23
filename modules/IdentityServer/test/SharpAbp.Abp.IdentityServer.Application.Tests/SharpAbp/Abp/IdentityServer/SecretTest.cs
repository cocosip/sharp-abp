using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SharpAbp.Abp.IdentityServer
{
    public class SecretTest
    {
        [Fact]
        public void Secret_Sha256_Test()
        {
            //IdentityServer4 中的sha256方法,计算的为secret的值
            var s1 = "1q2w3e*".Sha256();
            Assert.Equal("E5Xd4yMqjP5kjWFKrYgySBju6JVfCzMyFp7n2QmMrME=", s1);
        }

        [Fact]
        public void Tuple_Equal_Test()
        {
            var tuples1 = new List<(string, int)>()
            {
                ("1",100),
                ("2",200),
                ("3",300)
            };

            var tuples2 = new List<(string, int)>()
            {
                ("2",200),
                ("3",300),
                ("4",400),
                ("5",500)
            };


            var v = tuples1.Except(tuples2).ToList();
            Assert.Single(v);
            Assert.Equal(("1", 100), v.FirstOrDefault());
        }
    }
}
