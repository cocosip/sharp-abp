using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.AuditLogging
{
   public class AuditLogAppServiceTest : AuditLoggingApplicationTestBase
   {
       private readonly IAuditLogAppService _auditLogAppService;
       public AuditLogAppServiceTest()
       {
           _auditLogAppService = GetRequiredService<IAuditLogAppService>();
       }


       [Fact]
       public async Task Get_Paged_EntityChanges_Actions_Test()
       {
           //025A520D-9FE3-4226-B89D-FA542C3F2D6C

           var auditLogs = await _auditLogAppService.GetPagedListAsync(new AuditLogPagedRequestDto());

           Assert.Equal(1, auditLogs.TotalCount);
           Assert.Single(auditLogs.Items);

        //    var auditLogId = auditLogs.Items.FirstOrDefault().Id;
        //    var auditLog = await _auditLogAppService.GetAsync(auditLogId);

        //    Assert.Equal("AuditLoggingTest", auditLog.ApplicationName);

        //    Assert.Single(auditLog.Actions);

        //    var entityChangePageResult = await _auditLogAppService.GetEntityChangePagedListAsync(new EntityChangePagedRequestDto()
        //    {
        //        AuditLogId = auditLogId
        //        //EntityId = "025A520D-9FE3-4226-B89D-FA542C3F2D6C"
        //    });

        //    Assert.Equal(1, entityChangePageResult.TotalCount);
       }




   }
}
