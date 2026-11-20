using Microsoft.Extensions.Options;
using Volo.Abp.Auditing;
using Xunit;

namespace SharpAbp.Abp.FileStoringManagement;

public class ContainerAuditingTest
{
    [Fact]
    public void Container_Changes_Audit_Configuration_Names_Without_Provider_Credentials()
    {
        const string secret = "cloud-secret-must-not-be-audited";
        var input = new CreateOrUpdateContainerDto
        {
            Name = "orders-files", Title = "Order documents", Provider = "Aws",
            Items = [new CreateOrUpdateContainerItemDto("Aws.SecretAccessKey", secret)]
        };
        var serializer = new JsonAuditSerializer(Options.Create(new AbpAuditingOptions()));

        var parameters = serializer.Serialize(new { input });

        Assert.DoesNotContain(secret, parameters);
        Assert.Contains("orders-files", parameters);
        Assert.Contains("Order documents", parameters);
        Assert.Contains("Aws.SecretAccessKey", parameters);
    }
}
