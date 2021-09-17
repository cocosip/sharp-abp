using IdentityServer4.Models;
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
    }
}
