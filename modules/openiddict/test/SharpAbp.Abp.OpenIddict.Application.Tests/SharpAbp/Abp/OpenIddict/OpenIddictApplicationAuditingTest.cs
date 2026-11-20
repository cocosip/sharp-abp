using Microsoft.Extensions.Options;
using Volo.Abp.Auditing;
using Xunit;

namespace SharpAbp.Abp.OpenIddict;

public class OpenIddictApplicationAuditingTest
{
    [Fact]
    public void Application_Changes_Audit_Client_Metadata_Without_Client_Secrets()
    {
        var input = new CreateOrUpdateOpenIddictApplicationDto
        {
            ClientId = "orders-client", ClientType = "confidential", ClientSecret = "client-secret-must-not-be-audited"
        };
        var serializer = new JsonAuditSerializer(Options.Create(new AbpAuditingOptions()));

        var parameters = serializer.Serialize(new { input });

        Assert.DoesNotContain(input.ClientSecret, parameters);
        Assert.Contains("orders-client", parameters);
        Assert.Contains("confidential", parameters);
    }
}
