using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.SecurityLog;
using Xunit;

namespace SharpAbp.Abp.Identity
{
    public class IdentitySecurityLogAppServiceTest : IdentityApplicationTestBase
    {
        private readonly ISecurityLogStore _securityLogStore;
        private readonly IIdentitySecurityLogAppService _identitySecurityLogAppService;
        public IdentitySecurityLogAppServiceTest()
        {
            _securityLogStore = GetRequiredService<ISecurityLogStore>();
            _identitySecurityLogAppService = GetRequiredService<IIdentitySecurityLogAppService>();
        }

        [Fact]
        public async Task SecurityLog_Crud_TestAsync()
        {

            var securityLog1 = new SecurityLogInfo()
            {
                ApplicationName = "app1",
                Identity = "123",
                Action = "CreateUser",
                UserId = null,
                UserName = "",
            };

            var securityLog2 = new SecurityLogInfo()
            {
                ApplicationName = "app1",
                Identity = "456",
                Action = "UpdateUser",
                UserId = Guid.NewGuid(),
                UserName = "",
            };

            await _securityLogStore.SaveAsync(securityLog1);
            await _securityLogStore.SaveAsync(securityLog2);

            var pagedSecurityLog = await _identitySecurityLogAppService.GetPagedListAsync(new IdentitySecurityLogPagedRequestDto());

            Assert.Equal(2, pagedSecurityLog.TotalCount);
            Assert.Equal(2, pagedSecurityLog.Items.Count);


            var first = pagedSecurityLog.Items.FirstOrDefault();

            await _identitySecurityLogAppService.DeleteAsync(first.Id);

            var pagedSecurityLog2 = await _identitySecurityLogAppService.GetPagedListAsync(new IdentitySecurityLogPagedRequestDto());

            Assert.Equal(1, pagedSecurityLog2.TotalCount);


        }



    }
}
