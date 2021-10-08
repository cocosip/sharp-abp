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

            var user1 = await _identityUserAppService.FindByEmailAsync("123@qq.com");
            Assert.Equal("123@qq.com", user1.Email);
            Assert.Equal("admin1", user1.Name);

            await _identityUserAppService.LockAsync(user1.Id, 30);

            await _identityUserAppService.UnLockAsync(user1.Id);

            await _identityUserAppService.UpdateAsync(user.Id, new NewIdentityUserUpdateDto()
            {
                Password = "123Qwe*",
                LockoutEnabled = false,
                PhoneNumber = "18688888888",
                Name = "admin1",
                UserName = "zhangsan",
                Email = "123@qq.com",
            });

            await _identityUserAppService.DeleteAsync(user.Id);

        }

    }
}
