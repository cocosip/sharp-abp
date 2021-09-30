using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Identity
{
    public class IdentityUserAppServiceTest : IdentityApplicationTestBase
    {
        private readonly IIdentityUserAppService _identityUserAppService;
        private readonly Volo.Abp.Identity.IIdentityUserAppService _voloIdentityUserAppService;
        public IdentityUserAppServiceTest()
        {
            _identityUserAppService = GetRequiredService<IIdentityUserAppService>();
            _voloIdentityUserAppService = GetRequiredService<Volo.Abp.Identity.IIdentityUserAppService>();
        }

        [Fact]
        public async Task FindByUsername_TestAsync()
        {
            var user = await _identityUserAppService.CreateAsync(new NewIdentityUserCreateDto()
            {
                Name = "admin1",
                UserName = "zhangsan",
                Password = "1q2w3E*",
                Email = "123@qq.com",
            });

            Assert.Equal("zhangsan", user.UserName);
            Assert.Equal("admin1", user.Name);

        }

    }
}
