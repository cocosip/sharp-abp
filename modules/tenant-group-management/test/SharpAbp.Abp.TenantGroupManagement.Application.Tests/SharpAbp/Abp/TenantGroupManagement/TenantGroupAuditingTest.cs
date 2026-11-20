using System;
using Volo.Abp.Auditing;
using Xunit;

namespace SharpAbp.Abp.TenantGroupManagement;

public class TenantGroupAuditingTest : TenantGroupManagementTestBase
{
    [Theory]
    [InlineData(typeof(ITenantGroupAppService))]
    [InlineData(typeof(TenantGroupAppService))]
    [InlineData(typeof(TenantGroupController))]
    public void Connection_String_Updates_Keep_Audit_Metadata_Without_Credentials(Type serviceType)
    {
        const string connectionString = "Server=example;Password=secret-must-not-be-audited";
        var id = Guid.Parse("c6188e42-4385-491e-8d42-87045e747d4c");
        var method = serviceType.GetMethod(nameof(ITenantGroupAppService.UpdateDefaultConnectionStringAsync));
        var helper = GetRequiredService<IAuditingHelper>();

        Assert.True(helper.ShouldSaveAudit(method, defaultValue: true));
        var action = helper.CreateAuditLogAction(new AuditLogInfo(), serviceType, method, new object[] { id, connectionString });

        Assert.DoesNotContain("secret-must-not-be-audited", action.Parameters);
        Assert.Contains(id.ToString(), action.Parameters);
        Assert.Equal(nameof(ITenantGroupAppService.UpdateDefaultConnectionStringAsync), action.MethodName);
    }
}
