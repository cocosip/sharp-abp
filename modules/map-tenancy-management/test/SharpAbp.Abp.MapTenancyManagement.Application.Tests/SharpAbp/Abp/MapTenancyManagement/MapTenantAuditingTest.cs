using System;
using Microsoft.Extensions.Options;
using Volo.Abp.Auditing;
using Xunit;

namespace SharpAbp.Abp.MapTenancyManagement;

public class MapTenantAuditingTest : MapTenancyManagementApplicationTestBase
{
    [Fact]
    public void Tenant_Creation_Audits_Metadata_Without_Administrator_Passwords()
    {
        var input = new CreateHybridMapTenantDto
        {
            Name = "orders-tenant", Code = "orders", MapCode = "external-orders",
            AdminEmailAddress = "admin@example.com", AdminPassword = "admin-password-must-not-be-audited"
        };
        var serializer = new JsonAuditSerializer(Options.Create(new AbpAuditingOptions()));

        var parameters = serializer.Serialize(new { input });

        Assert.DoesNotContain(input.AdminPassword, parameters);
        Assert.Contains("orders-tenant", parameters);
        Assert.Contains("admin@example.com", parameters);
    }

    [Theory]
    [InlineData(typeof(IHybridMapTenantAppService))]
    [InlineData(typeof(HybridMapTenantAppService))]
    [InlineData(typeof(HybridMapTenantController))]
    public void Connection_String_Updates_Keep_Audit_Metadata_Without_Credentials(Type serviceType)
    {
        const string connectionString = "Server=example;Password=secret-must-not-be-audited";
        var id = Guid.Parse("c6188e42-4385-491e-8d42-87045e747d4c");
        var method = serviceType.GetMethod(nameof(IHybridMapTenantAppService.UpdateDefaultConnectionStringAsync));
        var helper = GetRequiredService<IAuditingHelper>();

        Assert.True(helper.ShouldSaveAudit(method, defaultValue: true));
        var action = helper.CreateAuditLogAction(new AuditLogInfo(), serviceType, method, new object[] { id, connectionString });

        Assert.DoesNotContain("secret-must-not-be-audited", action.Parameters);
        Assert.Contains(id.ToString(), action.Parameters);
        Assert.Equal(nameof(IHybridMapTenantAppService.UpdateDefaultConnectionStringAsync), action.MethodName);
    }
}
